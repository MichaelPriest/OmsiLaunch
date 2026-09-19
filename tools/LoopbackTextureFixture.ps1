param(
    [int]$Port = 18081,
    [Parameter(Mandatory = $true)][string]$PayloadPath,
    [Parameter(Mandatory = $true)][string]$LogPath
)

$listener = [System.Net.HttpListener]::new()
$listener.Prefixes.Add("http://127.0.0.1:$Port/")
$listener.Start()
try {
    while ($listener.IsListening) {
        $context = $listener.GetContext()
        $entry = [ordered]@{ utc = [DateTimeOffset]::UtcNow.ToString('O'); method = $context.Request.HttpMethod; raw_url = $context.Request.RawUrl; if_modified_since = $context.Request.Headers['If-Modified-Since']; if_none_match = $context.Request.Headers['If-None-Match'] }
        ($entry | ConvertTo-Json -Compress) | Add-Content -LiteralPath $LogPath
        $response = $context.Response
        $response.StatusCode = 200
        $response.ContentType = 'image/x-tga'
        $response.Headers['Last-Modified'] = 'Mon, 01 Jan 2024 00:00:00 GMT'
        $response.Headers['ETag'] = '"omsilaunch-black-32"'
        $bytes = [IO.File]::ReadAllBytes($PayloadPath)
        $response.ContentLength64 = $bytes.Length
        if ($context.Request.HttpMethod -ne 'HEAD') { $response.OutputStream.Write($bytes, 0, $bytes.Length) }
        $response.Close()
    }
}
finally { $listener.Close() }
