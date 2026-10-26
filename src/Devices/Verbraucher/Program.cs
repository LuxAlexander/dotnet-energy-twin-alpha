using Verbraucher;

// =====================================================================
//  Testprogramm für das Kühlhaus-Modell
//  Es nutzt nur die öffentliche Schnittstelle von Kuehlhaus:
//  Update, SetDoor, TurnOn, TurnOff, TriggerFault, ResetFault
//  und die Eigenschaften RoomTempC, CompressorOn, ElectricPowerKW,
//  EnergyKWh, DoorOpen, State, ActiveFault.
// =====================================================================

while (true)
{
    Console.WriteLine();
    Console.WriteLine("=== Kühlhaus-Tests ===");
    Console.WriteLine("1  Tür öffnen und schließen");
    Console.WriteLine("2  Kompressorausfall (bis Übertemperatur)");
    Console.WriteLine("3  Reset bei zu warmem Raum");
    Console.WriteLine("4  Aus- und Einschalten");
    Console.WriteLine("5  Tagesverbrauch bei 20 °C und 30 °C");
    Console.WriteLine("A  Alle Tests nacheinander");
    Console.WriteLine("Q  Beenden");
    Console.Write("Auswahl: ");

    string input = (Console.ReadLine() ?? "").Trim().ToUpperInvariant();
    if (input == "Q") return;

    switch (input)
    {
        case "1": Test1Door(); break;
        case "2": Test2CompressorFailure(); break;
        case "3": Test3ResetWhileWarm(); break;
        case "4": Test4OnOff(); break;
        case "5": Test5Energy(); break;
        case "A":
            Test1Door();
            Test2CompressorFailure();
            Test3ResetWhileWarm();
            Test4OnOff();
            Test5Energy();
            break;
        default:
            Console.WriteLine("Ungültige Auswahl.");
            break;
    }
}

// ---------------------------------------------------------------------
//  Test 1: Tür
// ---------------------------------------------------------------------
static void Test1Door()
{
    Header("Test 1: Tür (Außentemperatur 20 °C)",
        "Tür ist von 0:00 bis 1:00 offen.",
        "Erwartet: Mit offener Tür steigt die Raumtemperatur auf ca. 5 K/h statt 2 K/h.",
        "Der Kompressor schaltet früher ein (ab -18 °C) und kühlt wegen der Tür nur mit ca. 1 K/h.",
        "Nach dem Schließen kühlt er wieder normal bis -22 °C und schaltet ab.");

    Simulate(outsideC: 20, minutes: 6 * 60, printEveryMin: 15, events: (m, c) =>
    {
        if (m == 0) { c.SetDoor(true); Event(m, "Tür geöffnet"); }
        if (m == 60) { c.SetDoor(false); Event(m, "Tür geschlossen"); }
    });
}

// ---------------------------------------------------------------------
//  Test 2: Kompressorausfall
// ---------------------------------------------------------------------
static void Test2CompressorFailure()
{
    Header("Test 2: Kompressorausfall (Außentemperatur 20 °C)",
        "Ausfall des Kompressors bei 2:00.",
        "Erwartet: Kompressor bleibt aus, Zustand = Fault, Fehler = CompressorFailure.",
        "Der Raum steigt mit ca. 2 K/h. Nach ca. 6 bis 7 Stunden (um etwa 8:30 bis 9:00)",
        "wird -10 °C überschritten, und der Fehler wechselt auf OverTemperature.");

    Simulate(outsideC: 20, minutes: 12 * 60, printEveryMin: 30, events: (m, c) =>
    {
        if (m == 120)
        {
            c.TriggerFault(FaultType.CompressorFailure);
            Event(m, "Kompressorausfall ausgelöst");
        }
    });
}

// ---------------------------------------------------------------------
//  Test 3: Reset bei zu warmem Raum
// ---------------------------------------------------------------------
static void Test3ResetWhileWarm()
{
    Header("Test 3: Reset bei zu warmem Raum (Außentemperatur 20 °C)",
        "Ausfall bei 2:00, erster Reset bei 10:00, zweiter Reset bei 12:00.",
        "Erwartet: Beim ersten Reset läuft der Kompressor wieder an,",
        "aber der Alarm bleibt gespeichert (OverTemperature), solange der Raum über -10 °C liegt.",
        "Beim zweiten Reset ist der Raum kalt genug, und der Fehler wird auf None gesetzt.");

    Simulate(outsideC: 20, minutes: 14 * 60, printEveryMin: 30, events: (m, c) =>
    {
        if (m == 120)
        {
            c.TriggerFault(FaultType.CompressorFailure);
            Event(m, "Kompressorausfall ausgelöst");
        }
        if (m == 600)
        {
            c.ResetFault();
            Event(m, $"Reset 1 ausgeführt, Fehler jetzt: {c.ActiveFault}, Raum {c.RoomTempC:F2} °C");
        }
        if (m == 720)
        {
            c.ResetFault();
            Event(m, $"Reset 2 ausgeführt, Fehler jetzt: {c.ActiveFault}, Raum {c.RoomTempC:F2} °C");
        }
    });
}

// ---------------------------------------------------------------------
//  Test 4: Ein/Aus
// ---------------------------------------------------------------------
static void Test4OnOff()
{
    Header("Test 4: Aus- und Einschalten (Außentemperatur 20 °C)",
        "Gerät aus bei 1:00, wieder ein bei 4:00.",
        "Erwartet: Im ausgeschalteten Zustand zeigt der Zustand Off, der Kompressor ist aus,",
        "die Leistung ist 0, und der Raum erwärmt sich um ca. 2 K/h (bis ca. -12 °C, kein Alarm).",
        "Nach dem Einschalten läuft die Regelung wieder: Kompressor an, Zustand Cooling.");

    Simulate(outsideC: 20, minutes: 8 * 60, printEveryMin: 30, events: (m, c) =>
    {
        if (m == 60) { c.TurnOff(); Event(m, "Gerät ausgeschaltet"); }
        if (m == 240) { c.TurnOn(); Event(m, "Gerät eingeschaltet"); }
    });
}

// ---------------------------------------------------------------------
//  Test 5: Energie bei 20 °C und 30 °C
// ---------------------------------------------------------------------
static void Test5Energy()
{
    Header("Test 5: Tagesverbrauch über 24 Stunden",
        "Erwartet bei 20 °C: COP 2,0, Leistung 3,0 kW, Laufzeit ca. 33 %, Energie ca. 24 kWh.",
        "Erwartet bei 30 °C: COP 1,5, Leistung 4,0 kW, Laufzeit ca. 42 %, Energie ca. 40 kWh.");

    foreach (double outside in new[] { 20.0, 30.0 })
    {
        var cold = new Kuehlhaus(new KuehlhausConfig());
        int minutesOn = 0;
        double maxPower = 0;

        for (int i = 0; i < 24 * 60; i++)
        {
            cold.Update(60, outside);
            if (cold.CompressorOn) minutesOn++;
            if (cold.ElectricPowerKW > maxPower) maxPower = cold.ElectricPowerKW;
        }

        Console.WriteLine($"Außen {outside,4:F0} °C:  Energie {cold.EnergyKWh,6:F2} kWh   " +
                          $"max. Leistung {maxPower,4:F1} kW   Laufzeit {minutesOn / 14.40,5:F1} %");
    }
}

// ---------------------------------------------------------------------
//  Hilfsmethoden
// ---------------------------------------------------------------------

// Läuft Schritt für Schritt (1 Minute pro Schritt). Vor jedem Schritt
// wird 'events' mit der aktuellen Minute aufgerufen, dort lassen sich
// Tür, Ausfall, Reset usw. zu bestimmten Zeitpunkten auslösen.
static void Simulate(double outsideC, int minutes, int printEveryMin, Action<int, Kuehlhaus> events)
{
    var cold = new Kuehlhaus(new KuehlhausConfig());
    PrintLine(0, cold);

    for (int m = 0; m < minutes; m++)
    {
        events(m, cold);
        cold.Update(60, outsideC);

        int t = m + 1;
        if (t % printEveryMin == 0)
        {
            PrintLine(t, cold);
        }
    }
}

static void PrintLine(int minute, Kuehlhaus c)
{
    Console.WriteLine(
        $"{Clock(minute),5}  Raum {c.RoomTempC,7:F2} °C  " +
        $"Kompressor {(c.CompressorOn ? "AN " : "aus")}  " +
        $"Leistung {c.ElectricPowerKW,4:F1} kW  " +
        $"Energie {c.EnergyKWh,6:F2} kWh  " +
        $"Tür {(c.DoorOpen ? "offen" : "zu   ")}  " +
        $"{c.State,-8} Fehler: {c.ActiveFault}");
}

static void Event(int minute, string text)
{
    Console.WriteLine($">>> {Clock(minute),5}  {text}");
}

static string Clock(int minute) => $"{minute / 60}:{minute % 60:D2}";

static void Header(string title, params string[] expectations)
{
    Console.WriteLine();
    Console.WriteLine(new string('=', 78));
    Console.WriteLine(title);
    Console.WriteLine(new string('-', 78));
    foreach (string line in expectations)
    {
        Console.WriteLine(line);
    }
    Console.WriteLine(new string('-', 78));
}