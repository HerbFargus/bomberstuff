# Info #

Hier die nächste Alpha. Ich hoffe, die Installation und Benutzung ist etwas einfacher.
Freue mich über jegliches Feedback!

# Installation #
## Ihr braucht ##
  * .Net Framework (mindestens 2.0. In Windows Vista oder 7 bereits enthalten, und auf den meisten XP-Systemen wohl auch schon drauf) Den Download gibt es als [Web-Setup](http://www.microsoft.com/downloads/details.aspx?FamilyID=ab99342f-5d1a-413d-8319-81da479ab0d7) (2.8 MiB) oder [Komplettpaket](http://www.microsoft.com/downloads/details.aspx?familyid=D0E5DEA7-AC26-4AD7-B68C-FE5076BBA986) (231.5 MiB).
  * [SlimDX](http://slimdx.googlecode.com/files/SlimDX%20Runtime%20(February%202010).msi) (16.2 MiB) - neue Version. Die vom letzten Mal sollte aber auch klappen
  * Atomic Bomberman. Bei mir melden, wenn Ihr das noch nicht habt.

## Vorgehen ##
  * Voraussetzungen (oben) installieren. Merken, in welchem Ordner Atomic Bomberman liegt. z.B. `C:\Spiele\Atomic Bomberman`.
  * [Release 0.1.1](http://bomberstuff.googlecode.com/files/BomberStuff-alpha-0.1.1-bin.zip) von der [Download-Seite](http://code.google.com/p/bomberstuff/downloads/list) runterladen und in beliebigen Ordner entpacken, z.B. `C:\Spiele\BomberStuff`
  * `ConfigBM.exe` ausführen um Einstellungen zu machen. Den Atomic Bomberman-Pfad eintragen. Andere Einstellungen erstmal so lassen, können später bearbeitet werden.
  * `StartBM.exe` ausführen, um das Spiel zu starten
  * Spielen, rumprobieren, Einstellungen bearbeiten, mehr rumprobieren, ...

# What's new #
  * Einfache Konfiguration mittels Konfig-Dialog (`ConfigBM.exe`)
  * Einfacher Start mittels `StartBM.exe` (inkl. Logdatei-Erstellung)
  * Vernünftige Bewegung des Bomberman
  * Powerups (rudimentär)

# Was getestet werden soll #
  * Usability - Läuft's? Einrichten & Installieren zu kompliziert? Anleitung okay?
  * Steuerung & Bewegung - läuft es sich gut? Bleibt man irgendwie hängen?
  * Kollisionen - passiert irgendwas Merkwürdiges, wenn man wo gegen läuft? Funktioniert das Aufnehmen der Powerups richtig?
  * Bomben - kann man mehrere Bomben auf ein Feld legen? Explodiert alles so, wie es soll?
  * Einstellungen & Konfigurationsdialog - werden irgendwelche Einstellungen nicht beachtet, funktioniert irgendwas im Konfig-Dialog nicht?
  * [Farben](http://code.google.com/p/bomberstuff/issues/detail?id=1)! - Probiert mal aus, was toll aussieht. Zusätzliche Farben sind immer gut. Speziell weiß und gelb sehen auch noch etwas blöd aus
  * Performance - Wie viele FPS kriegt Ihr so?

# What's next #
Im nächsten Release könnt Ihr erwarten:
  * Verbesserte Konfiguration (Einstellen der Steuerungstasten, "Spiel starten"-Button, ...)
  * Schnellerer Spielstart
  * Netzwerk! :D

# Probleme, Fragen, Bugs #
Wenn etwas ein Bug zu sein scheint:
  * gucken, ob's unter "bekannte Probleme" steht
  * gucken, ob schon ein [Issue](http://code.google.com/p/bomberstuff/issues/list) vorhanden ist
  * [Issue posten](http://code.google.com/p/bomberstuff/issues/entry). Bei Konfigurationsgeschichten unbedlingt `settings.xml` anhängen, bei Abstürzen zusätzlich `BomberStuff.log`
  * macht überhaupt nichts, wenn sich rausstellt, dass es kein Bug war! Lieber zu viele als zu wenige.
Bei Ideen, Fragen, Problemen, am besten hier einen Kommentar posten. Nicht zögern, einfach schreiben :)<br>
Wer keinen Google-Account hat oder nutzen will, darf mir auch gerne per ICQ oder E-Mail schreiben.<br>
<br></li></ul>

<h1>Bekannte Probleme</h1>
<h2>Collision Detection</h2>
<ul><li>Wenn man gegen den Spielfeldrand läuft und gleichzeitig 'ne Taste zum seitlich laufen drückt, springt der etwas hin und her</li></ul>

<h2>Steuerung</h2>
<ul><li>Systemtasten wie Alt oder Windows-Taste gehen nicht oder nur begrenzt<br>
</li><li>Beim Gleichzeitigen drücken mehrerer Richtungen bewegt man sich nicht exakt wie in AB. (Stört das wen?)</li></ul>

<h2>Grafik</h2>
<ul><li>Animationen laufen nicht in korrekter Geschwindigkeit.<br>
</li><li>Powerups sind in einigen Pixeln transparent, die Schweine :p</li></ul>

<h2>Einstellungen</h2>
<ul><li>Fehler jeglicher Art in der XML-Konfiguration werden nicht abgefangen. Das Programm stürzt gnadenlos ab. Wenn Ihr unsicher seid, woran's liegt, XML-File und Fehlermeldung her und fragen :)</li></ul>

<h1>Tipps</h1>
<ul><li>Die Zeit, die das Spiel zum Laden braucht, ist proportional zur Spieleranzahl. Wenn's nicht um was mehrspielertechnisches geht, am besten nur einen Spieler einstellen.<br>
</li><li>Sämtliche auftretenden Exceptions ("Ausnahmen"?), die nicht durch ungültiges XML kommen, kopieren und als <a href='http://code.google.com/p/bomberstuff/issues/list'>Issue</a> posten. Bei <code>Assertion failed</code> wird das leider nicht in die Konsole geschrieben. Die oberste Quelldatei und Zeilennummer reichen mir, Ihr braucht nicht alles abtippen. Die Dinger sind aber besonders wichtig!<br>
</li><li>Die möglichen Einstellungen für die Steuerungstasten sind <a href='http://msdn.microsoft.com/de-de/library/system.windows.forms.keys(VS.80).aspx'>bei MSDN dokumentiert</a>