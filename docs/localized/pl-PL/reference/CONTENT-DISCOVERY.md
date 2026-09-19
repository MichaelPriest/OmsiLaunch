> Zlokalizowane tłumaczenie. W przypadku różnic technicznych obowiązuje kanoniczna dokumentacja angielska.

# Odkrywanie treści

Status: NORMATYWNY

Discovery jest w trybie offline i tylko do odczytu. Mapy używają `maps\...\global.cfg`; sytuacje
użyj `sytuacji\...\.osn`; pojazdy korzystają z plików `.bus` powiązanych z OMSI; przemalowania są
tożsamości CTI obejmujące zakres pojazdu. HOF, numer floty, rejestracja i dodatek
raport wyników zawiera wyłącznie dowody dostępne w zainstalowanej zawartości. Zamówienie odkrycia lub
znaczniki czasu plików nigdy nie definiują semantyki zapisanego stanu środowiska wykonawczego.
## Punkty wejścia

`EnumerateEntrypoints(mapIdentity)` analizuje `[punkty wejścia]` zaobserwowane w profilu
zapisy w `global.cfg`. Jego tożsamość kanoniczna to tożsamość mapy plus an
SHA-256 pełnego znormalizowanego zapisu surowego. Etykiety są metadanymi wyświetlanymi
tylko dlatego, że mapy OMSI mogą zawierać zduplikowane etykiety. Odkrycie nie twierdzi
że ten surowy rekord został skorelowany ze środowiskiem wykonawczym „Tform_setpos”.
prezentowana lista.

