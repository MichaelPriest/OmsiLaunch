> Zlokalizowane tłumaczenie. W przypadku różnic technicznych obowiązuje kanoniczna dokumentacja angielska.

# Katalog możliwości OmsiLaunch Beta 0.1

Wersja produktu: `0.1.0`  
Protokół kontrolny: `0.1`  
Obsługiwany profil wykonywalny: `Omsi23004_692EBFBF` (`Omsi.exe` SHA-256 `692EBFBF2CD32FAB05A8B934E52C2BE14594E939882F3DBF2BA4E2B66CCC6243`). Profilowany Steam LAA SHA-256 został zaakceptowany, ale oczekuje na weryfikację w środowisku wykonawczym Beta.

Znaczenie statusu:

- **Obsługiwane**: sprawdzone w czasie wykonywania na obsługiwanym profilu i odpowiednie dla semantycznego API Beta.
- **Eksperymentalne**: zaimplementowane i sprawdzone tam, gdzie zostało to określone, ale szczegóły profilu lub kształt interfejsu API mogą ulec zmianie w trakcie wersji beta.
- **Częściowy**: dostępny jest przydatny podzbiór; wymienione ograniczenia są istotne.
- **Jeszcze niedostępne**: celowo nieobecne w publicznym kontrakcie Beta.
- **Wewnętrzne**: prymitywna implementacja/test, a nie publiczne zobowiązanie do wersji beta.

| Zdolność | Stan | Stabilność API | Walidacja środowiska wykonawczego | Znane ograniczenie |
|---|---|---|---|---|
| Planowanie sesji, uruchomienie, status, oczekiwanie, zatrzymanie, zamknięcie | Obsługiwane | Stabilna wersja beta | Przełęcz | Tylko jeden dokładny profil OMSI. |
| Transakcyjne przemieszczanie i przywracanie środowiska wykonawczego/konfiguracji | Obsługiwane | Stabilna wersja beta | Przełęcz | Wszystkie zmiany konfiguracji uruchamiania mają zasięg sesji. |
| Uruchomienie NEW_MAP według prezentowanego indeksu punktu wejścia | Obsługiwane | Stabilna wersja beta | Przełęcz | Tożsamość semantycznego punktu wejścia pozostaje częściowa. |
| SAVED_SITUATION wysyłka natywna | Eksperymentalny | Eksperymentalny | Przełęcz | Testowana zapisana zawartość musi odpowiadać zainstalowanej mapie/treści. |
| LAST_MAP_STATE | Jeszcze niedostępne | Nie dotyczy | Nie prowadzony | Nigdy nie wywnioskowano z najnowszego `.osn`. |
| Odkrywanie treści i PlanSession | Obsługiwane | Stabilna wersja beta | Przepustka offline | Odkrywanie zależy od profilu/treści. |
| Odczyt czasu i ustawienie czasu natywnego | Obsługiwane | Stabilna wersja beta | Przełęcz | Mutacja Kalendarza/`SetActualDateTime` jest niedostępna. |
| Odczyt pogody i zapis skalarny na białej liście | Eksperymentalny | Eksperymentalny | Przełęcz | Rzeczywisty cykl życia konfiguracji/zastosowania ICAO jest niedostępny. |
| Odczyt kontrolera aktualnej pogody | Eksperymentalny | Eksperymentalny | Przełęcz | Brak publicznej operacji aktywacji/odświeżenia ICAO. |
| Podstawowy stan mapy odczytany | Obsługiwane | Stabilna wersja beta | Przełęcz | Zaawansowane pola metadanych mapy/wykresów kafelkowych są częściowe. |
| Odczyt z kamery i ograniczony zapis FOV | Eksperymentalny | Eksperymentalny | Przełęcz | Cykl życia aparatu/semantyka rodziny pozostają zależne od profilu. |
| Kolekcja pojazdów drogowych i migawki szczegółów | Eksperymentalny | Eksperymentalny | Przełęcz | Żadnych arbitralnych przemieszczeń przestrzennych ani ponownego łączenia ODE. |
| Odczyt/indeks PlayerVehicle | Obsługiwane | Stabilna wersja beta | Przełęcz | Pojazd gracza zerowego jest ważny; deterministyczne przypisanie bezgłowe jest niedostępne. |
| Ludzie i migawki z rozkładu jazdy | Eksperymentalny | Eksperymentalny | Przełęcz | Niektóre zaawansowane układy z bieżącej kompilacji pozostają częściowe. |
| Numeryczne zmienne skryptowe pojazdu | Eksperymentalny | Eksperymentalny | Odczyt/zapis przepustki | Zapis zmiennej łańcuchowej jest niedostępny. |
| Zmienna łańcuchowa, stałe, krzywe, HOF, kierowcy, bilety, odczytane logi | Eksperymentalny | Eksperymentalny | Przełęcz | Mutacja ciągu zarządzanego i nazwane wyzwalacze są niedostępne. |
| `pojazdy-drogowe.miejsce-losowe` | Eksperymentalny | Eksperymentalny | Przełęcz | Rodzima mutacja świata; zwracana wartość jest diagnostyczna, a nie tożsamościowa. |
| Podstawowy element MakeVehicle | Wewnętrzne | Tylko wewnętrzne | Przełęcz | Nie tworzy/przypisuje PlayerVehicle. |
| Status D3D i cykl życia tekstur | Eksperymentalny | Eksperymentalny | Przełęcz | Cykl życia związany z utratą urządzenia/resetem został udowodniony tylko częściowo. |
| Zdarzenia środowiska wykonawczego/sesji | Częściowe | Eksperymentalny | Częściowe | Zdarzenia utracone/zresetowane/przywrócone D3D nie mają prawdziwego dowodu na działanie. |
| Nakładki klawiatury/kontrolera/konfiguracji | Obsługiwane | Stabilna wersja beta | Karnet offline/integracyjny | Stosowane tylko dla sesji i przywracane bajt po bajcie. |

Wszystkie uchwyty środowiska wykonawczego są nieprzezroczyste i mają zasięg sesji. Nie są to wskaźniki OMSI, nie można ich ponownie wykorzystać po zakończeniu sesji i można je odrzucić jako nieaktualne lub zwolnione.

OmsiLaunch nie wymaga plików binarnych OmsiHook, wtyczki RPC ani plików wykonawczych. OmsiHook pozostaje wyrocznią inżynieryjną z udokumentowanym pochodzeniem LGPL; nie jest to zależność w czasie wykonywania.

