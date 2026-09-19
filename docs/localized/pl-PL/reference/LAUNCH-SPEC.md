> Zlokalizowane tłumaczenie. W przypadku różnic technicznych obowiązuje kanoniczna dokumentacja angielska.

# Specyfikacja uruchomienia

Status: NORMATYWNY

„LaunchSpec” to przenośne dane semantyczne. `UNSET` oznacza zachowanie istniejącego
Ustawienie OMSI i nigdy nie może być zwinięte do wartości „fałsz”, zero lub wymyślone
domyślny. Reprezentacja JSON jest tym samym modelem, którego używa `/spec`; wyraźne
Przełączniki CLI zastępują wartości plików.

Obsługiwane grupy strukturalne to instalacja, świat, tożsamość punktu wejścia lub
prezentowany indeks, data, godzina, rok, pogoda, pojazd gracza, konfiguracja,
dane wejściowe, diagnostyka i zachowanie w czasie wykonywania. `NOWA_MAPA`, `ZAPISANA_SYTUACJA` i
`LAST_MAP_STATE` to tryby semantyczne. „LAST_MAP_STATE” oznacza natywny stan OMSI
gałąź automatycznego przywracania ostatniej mapy; nigdy nie jest on wywnioskowany z pliku `.osn`
znaczniki czasu lub kolejność katalogów. Numeryczne indeksy list natywnych nie są
trwałe tożsamości treści.

Dla `NEW_MAP`, `World.EntrypointIdentity` jest zarezerwowane dla przyszłej struktury
tożsamość kanoniczna. Surowa etykieta `global.cfg` nie jest wystarczająca, ponieważ może
być powielane i mogą różnić się od natywnej prezentowanej etykiety. Aż do
profil zamyka tę korelację, obsługiwany jest „PresentedEntrypointIndex”.
selektor diagnostyczny/niskiego poziomu. Przekazanie uruchomienia v3 zawiera pole tożsamości
bez wartości wielkości wskaźnika, ale użytek publiczny pozostaje ograniczony możliwościami.

Data i godzina korzystają z wyraźnych rekordów „rok/miesiąc/dzień” i „godzina/minuta/sekunda”.
Schemat nie obejmuje wersji systemu operacyjnego, uchwytów Win32, obiektów wykonawczych CLR,
Szczegóły DNNE, mapowania pamięci lub adresy natywne.

PlanSession jest kompilatorem tych danych. Planu nie można wykonać tylko wtedy, gdy a
żądana wymagana funkcja jest niedostępna. Nieoczekiwane możliwości opcjonalne
bramki mają charakter informacyjny.

