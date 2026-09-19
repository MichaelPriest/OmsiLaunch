> Zlokalizowane tłumaczenie. W przypadku różnic technicznych obowiązuje kanoniczna dokumentacja angielska.

# Przenośność starszej wersji

Status: NORMATYWNY
Stan realizacji: PRZYSZŁY

Przenośność starszych wersji jest ograniczeniem projektowym, a nie bieżącym celem wdrożenia.

## Cel

Po ustabilizowaniu się wartości Current i zanim Current opuści początkową linię bazową platformy .NET,
zamrozić funkcjonalną bazę starszych portów dla dwóch jawnych backportów: Legacy NT6
i starsze XP.

## Zasada

Obecny system nie jest przestarzały, aby obsługiwać stary system Windows. Semantyka LaunchSpec, publiczna
wyniki/błędy, dane BuildProfile, tożsamość treści, semantyka konfiguracji,
protokół przewodowego przekazywania uruchamiania i natywne tożsamości operacyjne pozostają przenośne;
Poniżej znajdują się różne implementacje platformy.

`OmsiBuildProfile` jest niezależny od `RuntimePlatform`. DNNE jest prądem
adapter hosta wtyczki, a nie wymóg protokołu.

## Starsze cele

Starsza wersja systemu NT6 jest przeznaczona dla systemów Vista SP2 x64, Windows 7 SP1 x64, Windows 8 x64 i
Windows 8.1 x64. Legacy XP jest przeznaczony tylko dla systemu Windows XP SP3 x86. Rzeczywiste dziedzictwo
zestawy narzędzi są wybierane w ramach dedykowanych kamieni milowych portu.

## Kompatybilność z przewodami

Struktury przewodów startowych/sesji są wersjonowane; używaj pól o stałej szerokości, UTF-8
ciągi znaków o jawnych długościach i udokumentowanych zasadach pakowania/wyrównania/końca.
Nigdy nie używają serializacji obiektów CLR ani układu zależnego od rozmiaru wskaźnika.

## Powtarzalność

Tam, gdzie obsługuje to kompilacja/treść OMSI, powinna być taka sama semantyka LaunchSpec
możliwość odtwarzania w bieżących i starszych implementacjach platform w celach porównawczych i
badania optymalizacyjne.

## Sekwencja rozwoju

1. Uzupełnij i ustabilizuj prąd.
2. Zablokuj/oznacz `starszą bazę portów`.
3. Utwórz `legacy/nt6` i `legacy/xp`.
4. Wykonaj jawne backporty i zachowaj te gałęzie.
5. Zmodernizuj obecny dopiero wtedy, gdy poziom bazowy będzie istniał.

Implementacja starszej wersji nie rozpoczyna się podczas początkowej bieżącej kompilacji.

