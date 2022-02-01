# Changelog

All notable changes to this project will be documented in this file. See [standard-version](https://github.com/conventional-changelog/standard-version) for commit guidelines.

## [1.0.0](https://github.com/GetStream/stream-chat-net/compare/0.26.0...1.0.0) (2022-02-01)


### ⚠ BREAKING CHANGES

* The library received many changes in v1.0 to make it easier to use and more maintanable in the future.
The main change is that both [`Channel`](https://github.com/GetStream/stream-chat-net/blob/0.26.0/src/stream-chat-net/Channel.cs) and [`Client`](https://github.com/GetStream/stream-chat-net/blob/0.26.0/src/stream-chat-net/Client.cs) classes have been separated into small modules that we call clients. (This resambles the [structure of our Java library](https://github.com/GetStream/stream-chat-java/tree/1.5.0/src/main/java/io/getstream/chat/java/services) as well.)
Main changes:
- `Channel` and `Client` classes are gone, and have been organized into smaller clients in `StreamChat.Clients` namespace.
- These clients do not maintain state as `Channel` used to did earlier where [it kept the `channelType` and `channelId` in the memory](https://github.com/GetStream/stream-chat-net/blob/0.26.0/src/stream-chat-net/Channel.cs#L34-#L35). So this means that you'll need to pass in `channelType` and `channelId` to a lot of method calls in `IChannelClient`.
- Async method names have `Async` suffix now.
- All public methods and classes have documentation.
- Identifiers has been renamed from `ID` to `Id` to follow [Microsoft's naming guide](https://docs.microsoft.com/en-us/dotnet/standard/design-guidelines/capitalization-conventions). Such as `userID` -> `userId`.
- A lot of data classes have been renamed to make more sense. Such as `ChannelObject` -> `Channel`.
- Data classes have been moved to `StreamChat.Models` namespace.
- Full feature parity: all backend APIs are available.
- Returned values are type of `ApiResponse` and expose rate limit informaiton with `GetRateLimit()` method.
- The folder structure of the project has been reorganized to follow [Microsoft's recommendation](https://gist.github.com/davidfowl/ed7564297c61fe9ab814).
- Unit tests have been improved. They are smaller, more focused and have cleanup methods.
- Added .NET 6.0 support.

### Features

* major refactor with breaking changes ([#92](https://github.com/GetStream/stream-chat-net/issues/92)) ([1810ea2](https://github.com/GetStream/stream-chat-net/commit/1810ea2df099de26eb8483c40e407b6639b55722))

## [0.26.0](https://github.com/GetStream/stream-chat-net/compare/0.25.0...0.26.0) (2021-12-22)


### Features

* add hidehistory to add members ([25e79c1](https://github.com/GetStream/stream-chat-net/commit/25e79c1414fc88b88aa8d140a4c57e709a7633ff))
* add permissions v2 ([#83](https://github.com/GetStream/stream-chat-net/issues/83)) ([26fb0f5](https://github.com/GetStream/stream-chat-net/commit/26fb0f50319b9d83814ef7a762dd60ab6526b0fc))

## [0.25.0](https://github.com/GetStream/stream-chat-net/compare/0.24.0...0.25.0) (2021-12-15)


### Features

* add iat support to token creation ([#84](https://github.com/GetStream/stream-chat-net/issues/84)) ([c1775e5](https://github.com/GetStream/stream-chat-net/commit/c1775e52f75c059508b3caba97d8a8a8afac4c34))
* add options to truncate ([#81](https://github.com/GetStream/stream-chat-net/issues/81)) ([71f4797](https://github.com/GetStream/stream-chat-net/commit/71f47976902c8cdd9ff4a063218d92e19b95f200))
* extend appsettings ([#87](https://github.com/GetStream/stream-chat-net/issues/87)) ([cbde797](https://github.com/GetStream/stream-chat-net/commit/cbde797a4ad4f6a03cc940da8ba4ee17e4d27b39))

## [0.24.0](https://github.com/GetStream/stream-chat-net/compare/0.23.0...0.24.0) (2021-12-06)


### Features

* add partial message update ([#77](https://github.com/GetStream/stream-chat-net/issues/77)) ([a3ecf14](https://github.com/GetStream/stream-chat-net/commit/a3ecf14e425ee14bae321d25b1300990685cf58a))

## 0.22.0 - 2021-11-16

- Add support for GetTask, DeleteChannels and DeleteUsers endpoint [#64](https://github.com/GetStream/stream-chat-net/pull/64)

## 0.21.0 - 2021-11-11

- Add skip_push option

## 0.20.1 - 2021-11-02

- Fix nuget release key encryption

## 0.20.0 - 2021-11-01

- Add async url enrich app config flag

## 0.19.0 - 2021-08-19

- Set base url to edge.
  - There is no need to set api location anymore so location from client options is removed.

## 0.18.0 - 2021-08-19

- Add get message by id support
- Fix some doc examples in readme

## 0.17.0 - 2021-07-19

- Add channel export support

## 0.16.0 - 2021-06-25

- Add search support

## 0.15.0 - 2021-06-17

- Expose error codes in the stream exception

## 0.14.0 - 2021-06-01

- Add flag user and message moderation support

## 0.13.0 - 2021-05-31

- Add support for channel hide/show
- Add support for silent messages
- Bump dependencies (`Newtonsoft.Json`) to the latest

## 0.12.0 - 2021-05-10

- Add missing fields of Device; `CreatedAt`, `Disabled` and `DisabledReason`.

## 0.11.0 - 2021-03-10

- Add GetRateLimits endpoint support.

## 0.10.0 - 2021-03-03

- Add MML support into message.

## 0.9.0 - 2021-03-03

- Add .Net Core 2, 3 and 5 support.

## 0.8.1 - 2021-02-02

- Use `ID` instead of `Id` to be consistent in user GDPR options
- Update copyright year

## 0.8.0 - 2021-02-02

- Add missing GDPR options and fix examples in readme
- Start a changelog
