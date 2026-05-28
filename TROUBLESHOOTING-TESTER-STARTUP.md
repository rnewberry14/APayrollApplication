# ClearPath Payroll Tester Startup Troubleshooting

Tester package only. Do not use for real payroll. Do not enter real SSNs, bank account numbers, EINs, API keys, or live payroll data.

## App Will Not Start

1. Confirm the package was unzipped first.
2. Open the extracted folder.
3. Double-click `START-ClearPathPayroll.bat`.
4. Keep the app window open while testing.
5. If the launcher says the app did not answer, open `ClearPathPayroll-startup.log` in the same folder.
6. Take a screenshot of the error or copy the last lines of the log into your feedback.

## Browser Does Not Open

Open your browser manually and enter:

```text
http://localhost:5080
```

If that does not work, check the app window for another local address and copy that address into your browser.

If `START-ClearPathPayroll.bat` says the app did not answer within 60 seconds, the app probably failed during startup. Open `ClearPathPayroll-startup.log` and send the error text back with your feedback.

## Port Already In Use

The launcher uses:

```text
http://localhost:5080
```

If the app says the port is already in use:

1. Close any other ClearPath Payroll app windows.
2. Close old command windows from earlier test runs.
3. Run `START-ClearPathPayroll.bat` again.
4. If the message continues, restart Windows and try again.

## Missing .NET Runtime

If Windows says .NET is missing, install the .NET 8 ASP.NET Core Runtime from Microsoft, then run `START-ClearPathPayroll.bat` again.

Use the official Microsoft .NET download page. Do not install runtime files from unknown sources.

## Startup Log

The launcher writes startup messages to:

```text
ClearPathPayroll-startup.log
```

This file is in the same folder as `START-ClearPathPayroll.bat`. It helps identify startup problems such as a missing runtime, a blocked port, or a database startup error.

## Windows Security Warning

Windows may show a warning because this is a local tester package.

1. Confirm the package came from the expected sender.
2. Confirm the folder name is the tester package you expected.
3. Continue only if you trust the package source.

## Reset Demo Data

1. Close the app.
2. Double-click `RESET-DEMO-DATA.bat`.
3. Type `Y` when prompted.
4. Start the app again.
5. Open `/demo/seed-data` and click `Create Demo Data`.

The reset file only removes local demo database files inside the extracted app folder.

## Send Error Screenshots

Send:

- The screenshot of the error.
- The page or button you clicked.
- The time the error happened.
- Your Windows version if known.

## What Not To Send

Do not send:

- SSNs.
- Bank information.
- Passwords.
- API keys.
- Real EINs.
- Live payroll data.
