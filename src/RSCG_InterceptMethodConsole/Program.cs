using RSCG_InterceptMethodConsole;
Person person = new ();
person.FirstName = "Andrei";
person.LastName = "Ignat";
Console.WriteLine(await person.FullName(5000) + await person.FullName(3000));