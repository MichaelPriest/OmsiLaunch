> Zlokalizowane tłumaczenie. W przypadku różnic technicznych obowiązuje kanoniczna dokumentacja angielska.

# Wsparcie platformy

Status: NORMATYWNY

## Prąd OmsiLaunch

Oficjalne Obecne wsparcie jest celowo wąskie: końcowe/ostatnie serwisowane
Windows 10 x86-64 i aktualnie obsługiwany system Windows 11 x86-64. System operacyjny hosta i
zewnętrzny Obecny host to AMD64/x86-64; OMSI, jego wtyczka procesowa i natywna
Współpraca OMSI pozostaje x86.

Obecny nie obsługuje 32-bitowych systemów Windows, ARM64, Vista, Windows 7, Windows 8,
Windows 8.1, Windows XP, Wine, Proton, Linux lub macOS. Nieobsługiwane platformy
są odrzucane podczas walidacji przed jakąkolwiek mutacją instalacyjną.

## Zależności środowiska wykonawczego

Końcowa dystrybucja prądu powinna być niezależna, jeśli jest to praktyczne. Użytkownicy tak
nie potrzebujesz Visual Studio, .NET SDK, Git, Python, Ghidra ani CMake. Normalne
operacja nie wymaga podniesienia poziomu, gdy instalacja OMSI jest zapisywalna
bieżącego użytkownika.

## Starsze rodziny

Starsze rodziny to osobne porty historyczne/badawcze, a nie bieżące wsparcie.

### Starsza wersja NT6

Zamierzone cele to ostatecznie poprawione systemy Windows Vista SP2 x64, Windows 7 SP1 x64,
Windows 8 x64 i Windows 8.1 x64.

### Starsza wersja XP

Zamierzonym celem jest Windows XP SP3 x86. Windows XP x64 nie jest celem.

## Pozycjonowanie zabezpieczające

Starsze wersje są przeznaczone dla systemów offline, mają zgodność historyczną i są kontrolowane
benchmarki i badania powtarzalności. Nie są zalecane dla normalnych
korzystanie z połączenia z Internetem.

