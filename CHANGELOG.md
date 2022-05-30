# Changelog

All notable changes to this project will be documented in this file. See [standard-version](https://github.com/conventional-changelog/standard-version) for commit guidelines.

## [2.1.0](https://github.com/GetStream/stream-chat-net/compare/2.0.0...2.1.0) (2022-05-30)


### Features

* add apn template field ([#111](https://github.com/GetStream/stream-chat-net/issues/111)) ([9fbd648](https://github.com/GetStream/stream-chat-net/commit/9fbd648db9e9ec6ec684c4caf46b4a2cd1e53034))
* add offlineonly to appconfig pushconfig ([#109](https://github.com/GetStream/stream-chat-net/issues/109)) ([c65bed4](https://github.com/GetStream/stream-chat-net/commit/c65bed4a002439039ed63b2e4382f24a8bbef91f))
* **imports:** add import endpoints ([#113](https://github.com/GetStream/stream-chat-net/issues/113)) ([6025c64](https://github.com/GetStream/stream-chat-net/commit/6025c64e51ab1c1d59550f147300d62c0e95f204))

## [2.0.0](https://github.com/GetStream/stream-chat-net/compare/1.1.1...2.0.0) (2022-04-06)


### ⚠ BREAKING CHANGES

* remove json property attributes (#100)

### Features

* add command override ([#103](https://github.com/GetStream/stream-chat-net/issues/103)) ([23669b8](https://github.com/GetStream/stream-chat-net/commit/23669b8dfca864a6faa41bc89f97f078ef1f1f7e))
* add device and push fields ([#104](https://github.com/GetStream/stream-chat-net/issues/104)) ([bc2a164](https://github.com/GetStream/stream-chat-net/commit/bc2a164bb99a28b80350b0b9ef455ceaec95d873))
* add new moderation apis ([#101](https://github.com/GetStream/stream-chat-net/issues/101)) ([f11a65f](https://github.com/GetStream/stream-chat-net/commit/f11a65f2f576a483cfbf3bf39fbb550b4952b3f5))
* add push provider api ([#102](https://github.com/GetStream/stream-chat-net/issues/102)) ([9ee847c](https://github.com/GetStream/stream-chat-net/commit/9ee847c7e3a6bf382948e072a92c9ffd03fc4c0e))
* add reminders ([#105](https://github.com/GetStream/stream-chat-net/issues/105)) ([2f551f1](https://github.com/GetStream/stream-chat-net/commit/2f551f1bb35e818c199899fc428587accaf130e4))
* add user custom events ([#96](https://github.com/GetStream/stream-chat-net/issues/96)) ([e94416a](https://github.com/GetStream/stream-chat-net/commit/e94416ac6b957c52006620f23ab5e9002236b348))
* **truncate:** add user id for truncation ([#107](https://github.com/GetStream/stream-chat-net/issues/107)) ([406c994](https://github.com/GetStream/stream-chat-net/commit/406c99409b081c857e11cc867126b61fa34a018b))


* remove json property attributes ([#100](https://github.com/GetStream/stream-chat-net/issues/100)) ([acf4b43](https://github.com/GetStream/stream-chat-net/commit/acf4b43021899bf28d3ba17f84c98563074142d0))

### [1.1.1](https://github.com/GetStream/stream-chat-net/compare/1.1.0...1.1.1) (2022-02-14)


### Bug Fixes

* fix property name for channel reads ([#97](https://github.com/GetStream/stream-chat-net/issues/97)) ([647e4f2](https://github.com/GetStream/stream-chat-net/commit/647e4f2d11f29dc5d7efb243351be3ab1b3d359d))

## [1.1.0](https://github.com/GetStream/stream-chat-net/compare/1.0.0...1.1.0) (2022-02-08)


### Features

* add helper methods for user invitation, acceptance and rejection ([#94](https://github.com/GetStream/stream-chat-net/issues/94)) ([491325f](https://github.com/GetStream/stream-chat-net/commit/491325f9363960b0490aefbda77555c2fcab3d3f))

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
