# Info #

Die erste Alpha-Version für Euch zum Testen ist da!<br>
Das Ganze ist noch nicht wirklich spielbar und enthält einige Problemchen, aber ein paar Sachen lohnen sich schon zu testen. Hauptsächlich, ob Ihr die Sache überhaupt zum Laufen kriegt.<br>
Hier findet sich die Installationsanleitung, Anmerkungen, was Ihr testen solltet und die schon bekannten Probleme.<br>
Sollte jetzt ca. wöchentlich 'ne neue Alpha geben, wenn ich dazu komme und genug Feedback da ist.<br>
Danke!<br>
<br>
<h1>Installation</h1>
Ihr braucht:<br>
<ul><li>.Net Framework (mindestens 2.0. In Windows Vista oder 7 bereits enthalten, und auf den meisten XP-Systemen wohl auch schon drauf) Den Download gibt es als <a href='http://www.microsoft.com/downloads/details.aspx?FamilyID=ab99342f-5d1a-413d-8319-81da479ab0d7'>Web-Setup</a> (2.8 MiB) oder <a href='http://www.microsoft.com/downloads/details.aspx?familyid=D0E5DEA7-AC26-4AD7-B68C-FE5076BBA986'>Komplettpaket</a> (231.5 MiB).<br>
</li><li><a href='http://slimdx.googlecode.com/files/SlimDX%20Runtime%20(August%202009).msi'>SlimDX</a> (16.1 MiB)<br>
</li><li>Atomic Bomberman. Bei mir melden, wenn Ihr das noch nicht habt.</li></ul>

Vorgehen:<br>
<ul><li>Voraussetzungen (oben) installieren. Merken, in welchem Ordner Atomic Bomberman liegt. z.B. <code>C:\Spiele\Atomic Bomberman</code>.<br>
</li><li>Release 0.1.0 (nach Belieben <a href='http://bomberstuff.googlecode.com/files/BomberStuff-alpha-0.1.0a-bin.zip'>Binär-</a> oder <a href='http://bomberstuff.googlecode.com/files/BomberStuff-alpha-0.1.0-all.zip'>Komplettpaket</a>) von der <a href='http://code.google.com/p/bomberstuff/downloads/list'>Download-Seite</a> runterladen und in beliebigen Ordner entpacken, z.B. <code>C:\Spiele\BomberStuff</code>
</li><li>settings.xml bearbeiten. Hier muss <code>ABDirectory</code> auf den Atomic Bomberman-Pfad (das Originalspiel!) zeigen. Z.B.:<br>
<pre><code>  &lt;!-- Set your Atomic Bomberman path here. It should contain a DATA subdirectory--&gt;<br>
  &lt;String name="ABDirectory"&gt;<br>
  	C:\Spiele\Atomic Bomberman<br>
  &lt;/String&gt;<br>
</code></pre>
</li><li>Andere Einstellungen erstmal so lassen, können später bearbeitet werden.<br>
</li><li>Von der Kommandozeile (!) aus <code>BomberStuff.exe</code> starten. Beim Komplettpaket müsst ihr in den Unterordner bin\Debug<br>
<pre><code>  cd/d C:\Spiele\BomberStuff<br>
  BomberStuff.exe<br>
</code></pre>
</li><li>Kommandozeile ist wichtig, weil dort Debug- und Fehlermeldungen stehen. Nach Belieben die Ausgabe in eine Datei umleiten (z.B. <code>BomberStuff.exe 2&gt;&amp;1 &gt;&gt; BomberStuff.log</code>)<br>
</li><li>Spielen, rumprobieren, settings.xml bearbeiten, mehr rumprobieren, ...</li></ul>

<h1>Bekannte Probleme</h1>
<h2>Collision Detection</h2>
<ul><li>man wird auf den Feldern noch nicht zentriert, d.h. man muss exakt laufen um durch Lücken zu kommen<br>
</li><li>man kann durch Explosionen laufen (wer im Weg steht, wenn die Bombe gezündet wird, stirbt, wer in die Explosion reinläuft, nicht)</li></ul>

<h2>Steuerung</h2>
<ul><li>Systemtasten wie Alt oder Windows-Taste gehen wahrscheinlich nicht oder nur begrenzt<br>
</li><li>das Loslassen einer Taste hält den Bomber an. Also immer schön Taste drücken, loslassen, drücken, loslassen etc., sonst bleibt er unerwartet stehen</li></ul>

<h2>Grafik</h2>
<ul><li>die Dauer von Explosionen stimmt wohl nicht. Die Mauer-Explodiert-Animation wird doppelt abgespielt<br>
</li><li>Wenn mehrere Sachen grafisch gesehen übereinander sind (Spieler & Bombe, Explosion & fast drinstehender Spieler... gibt's noch mehr Situationen?), ist es ziemlich zufällig (quasi genau verkehrt herum), was "vorne" ist, also das andere überdeckt</li></ul>

<h2>Einstellungen</h2>
<ul><li>Das Validieren der XML klappt noch nicht richtig. Der gibt halt irgendnen Unsinns-Fehler statt zu sagen Dokument ungültig<br>
</li><li>XML-Fehler jeglicher Art werden nicht abgefangen. Das Programm stürzt gnadenlos ab. Wenn Ihr unsicher seid, woran's liegt, XML-File und Fehlermeldung her und fragen :)</li></ul>

<h1>Was getestet werden soll</h1>
<ul><li>Steuerung, Bewegung, Kollisionserkennung (von bekannten Problemen abgesehen)<br>
</li><li>Bomben. Wie sich andere Sachen zerstören, wie sich mehrere Bomben miteinander verhalten, ...<br>
</li><li>Einstellungen. Ruhig mal ordentlich in der settings.xml rumspielen. Aber Vorsicht, XML+XSD=penibel. ungültige Werte sorgen dafür, dass das Spiel den Start verweigert<br>
</li><li><a href='http://code.google.com/p/bomberstuff/issues/detail?id=1'>Farben</a>! Probiert mal aus, was toll aussieht. Wie die Farben funktionieren, ist in der settings.xml erklärt. Speziell weiß und gelb sehen noch etwas blöd aus<br>
</li><li>gerne auch eigene Animationen/Hintergründe, die von irgendwo runtergeladen sind. In der MASTER.ALI eintragen (wie in AB)</li></ul>

<h1>Was NICHT all zu sinnvoll zu testen ist</h1>
<ul><li>Das zweite User-Interface (WinFormsInterface). Das wurde ziemlich vernachlässigt<br>
</li><li>Timings. Die sind grob eingestellt um was zum Testen zu haben</li></ul>

<h1>Probleme, Fragen, Bugs</h1>
Wenn etwas ein Bug zu sein scheint:<br>
<ul><li>gucken, ob's unter "bekannte Probleme" steht<br>
</li><li>gucken, ob schon ein <a href='http://code.google.com/p/bomberstuff/issues/list'>Issue</a> vorhanden ist<br>
</li><li><a href='http://code.google.com/p/bomberstuff/issues/entry'>Issue posten</a>
</li><li>macht überhaupt nichts, wenn sich rausstellt, dass es kein Bug war! Lieber zu viele als zu wenige.<br>
<br>
Bei Ideen, Fragen, Problemen, am besten hier einen Kommentar posten. Nicht zögern, einfach schreiben :)</li></ul>

<h1>Tipps</h1>
<ul><li>Die Zeit, die das Spiel zum Laden braucht, ist proportional zur Spieleranzahl. Wenn's nicht um was mehrspielertechnisches geht, am besten nur einen Spieler einstellen.<br>
</li><li>Sämtliche auftretenden Exceptions ("Ausnahmen"?), die nicht durch ungültiges XML kommen, kopieren und als <a href='http://code.google.com/p/bomberstuff/issues/list'>Issue</a> posten. Bei <code>Assertion failed</code> wird das leider nicht in die Konsole geschrieben. Die oberste Quelldatei und Zeilennummer reichen mir, Ihr braucht nicht alles abtippen. Die Dinger sind aber besonders wichtig!<br>
</li><li>Wenn Ihr die XML mit Visual Studio bearbeitet, gibt der gleich Warnungen, falls das nicht zum Schema passt. Andere gute XML-Editoren machen das natürlich auch...<br>
</li><li>Die möglichen Einstellungen für die Steuerungstasten sind <a href='http://msdn.microsoft.com/de-de/library/system.windows.forms.keys(VS.80).aspx'>bei MSDN dokumentiert</a>