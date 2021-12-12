# :recycle: Contributing

We welcome code changes that improve this library or fix a problem, please make sure to follow all best practices and add tests if applicable before submitting a Pull Request on Github. We are very happy to merge your code in the official repository. Make sure to sign our [Contributor License Agreement (CLA)](https://docs.google.com/forms/d/e/1FAIpQLScFKsKkAJI7mhCr7K9rEIOpqIDThrWxuvxnwUq2XkHyG154vQ/viewform) first. See our license file for more details.

## Getting started

### Restore dependencies and build

Most IDEs automatically prompt you to restore the dependencies but if not, use this command:

```shell
$ dotnet restore ./src
```

Building:

```shell
$ dotnet build ./src
```

### Run tests

The tests we have are full fledged integration tests, meaning they will actually reach out to a Stream app. Hence the tests require at least two environment variables: `STREAM_API_KEY` and `STREAM_API_SECRET`.

To have these env vars available for the test running, you need to set up a `.runsettings` file in the root of the project (don't worry, it's gitignored).

> :bulb Microsoft has a super detailed [documentation](https://docs.microsoft.com/en-us/visualstudio/test/configure-unit-tests-by-using-a-dot-runsettings-file) about .runsettings file.
  
It needs to look like this:

```xml
<?xml version="1.0" encoding="utf-8"?>
<RunSettings>
  <RunConfiguration>
    <EnvironmentVariables>
      <STREAM_API_KEY>api_key_here</STREAM_API_KEY>
      <STREAM_API_SECRET>secret_key_here</STREAM_API_SECRET>
    </EnvironmentVariables>
  </RunConfiguration>
</RunSettings>
```

In CLI:
```shell
$ dotnet test -s .runsettings ./src/stream-chat-net-test
```

Go to the next section to see how to use it in IDEs.

## Recommended tools for day-to-day development

### VS Code

For VS Code, the recommended extensions are:
- [C#](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.csharp) by Microsoft
- [.NET Core Test Explorer](https://marketplace.visualstudio.com/items?itemName=formulahendry.dotnet-test-explorer) by Jun Huan

Recommended settings (`.vscode/settings.json`):
```json
{
    "omnisharp.testRunSettings": ".runsettings",
    "dotnet-test-explorer.testProjectPath": "./src/stream-chat-net-test",
}
```

### Visual Studio

Follow [Microsoft's documentation](https://docs.microsoft.com/en-us/visualstudio/test/configure-unit-tests-by-using-a-dot-runsettings-file?view=vs-2022#specify-a-run-settings-file-in-the-ide) on how to set up `.runsettings` file.

### Rider

Follow [Jetbrain's documentation](https://www.jetbrains.com/help/rider/Reference__Options__Tools__Unit_Testing__MSTest.html) on how to set up `.runsettings` file.
