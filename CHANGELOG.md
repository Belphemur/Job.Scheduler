## [3.1.7](https://github.com/Belphemur/Job.Scheduler/compare/v3.1.6...v3.1.7) (2023-12-14)


### Bug Fixes

* **JobRunner:** Be sure that all jobs are run into tasks ([7bb4ac8](https://github.com/Belphemur/Job.Scheduler/commit/7bb4ac8e773f52a168a01051a02bfaf5435d5d91))

## [3.1.6](https://github.com/Belphemur/Job.Scheduler/compare/v3.1.5...v3.1.6) (2023-02-09)


### Bug Fixes

* **Debounce:** Be sure to only remove the debouncing key when the DebounceJob has succeeded, not when it was debounced. ([33bb8ff](https://github.com/Belphemur/Job.Scheduler/commit/33bb8ff016610845ea1c27657ee6d595e6d351c4))

## [3.1.5](https://github.com/Belphemur/Job.Scheduler/compare/v3.1.4...v3.1.5) (2023-02-09)


### Bug Fixes

* **Job:** fix possible null case when stopping a job ([cf7efc6](https://github.com/Belphemur/Job.Scheduler/commit/cf7efc6c949fabeb1c4323c29da2915c5fb5cfd0))

## [3.1.4](https://github.com/Belphemur/Job.Scheduler/compare/v3.1.3...v3.1.4) (2023-02-09)


### Bug Fixes

* **debounce:** Be sure to interrupt old running job when debouncing ([a45735c](https://github.com/Belphemur/Job.Scheduler/commit/a45735cde9c51cb38e89351daa0daf68587cc333))

## [3.1.3](https://github.com/Belphemur/Job.Scheduler/compare/v3.1.2...v3.1.3) (2023-02-05)


### Bug Fixes

* **debounce:** Be sure only the last job is ran in case of debounce. ([ae8e50a](https://github.com/Belphemur/Job.Scheduler/commit/ae8e50af57dea844d6b7ae25b6326a609f2233d1))

## [3.1.2](https://github.com/Belphemur/Job.Scheduler/compare/v3.1.1...v3.1.2) (2023-02-05)


### Performance Improvements

* **DebounceJob:** Improve the logic of the debounce job ([fed865b](https://github.com/Belphemur/Job.Scheduler/commit/fed865bd3433120baaad4a8366ce27fe23bb1244))

## [3.1.1](https://github.com/Belphemur/Job.Scheduler/compare/v3.1.0...v3.1.1) (2022-11-27)


### Bug Fixes

* **Scope::Dispose:** Fix issue where we dispose the scope too early ([f23e9ae](https://github.com/Belphemur/Job.Scheduler/commit/f23e9ae05a339e77b95aec93e6539e5040f7ecf3))

# [3.1.0](https://github.com/Belphemur/Job.Scheduler/compare/v3.0.2...v3.1.0) (2022-11-27)


### Features

* **Scope:** Create a new scope every time we build a job ([6ae54b8](https://github.com/Belphemur/Job.Scheduler/commit/6ae54b85922af76d165fa8c8882c0cf16c8ef199))

## [3.0.2](https://github.com/Belphemur/Job.Scheduler/compare/v3.0.1...v3.0.2) (2022-10-05)


### Bug Fixes

* **Scope:** Fix issue with scope being discarded and we still need access to the JobKey ([55a495f](https://github.com/Belphemur/Job.Scheduler/commit/55a495f65bb73274786c1d9ffc8f913abf8cc12a))

## [3.0.1](https://github.com/Belphemur/Job.Scheduler/compare/v3.0.0...v3.0.1) (2022-10-04)


### Bug Fixes

* **Scope:** Be sure to register job as transient ([dac2828](https://github.com/Belphemur/Job.Scheduler/commit/dac2828d45218a88ff851f808786ac4b839917d5))

# [3.0.0](https://github.com/Belphemur/Job.Scheduler/compare/v2.9.0...v3.0.0) (2022-10-04)


### Features

* **Scope:** Respect the scope of the job by building it on-demand and always rebuilding it for recurring jobs ([bd623f4](https://github.com/Belphemur/Job.Scheduler/commit/bd623f40825e048f75cb006b1564e40b43f0b6b1))


### BREAKING CHANGES

* **Scope:** The scheduling API is now type with generic. It shouldn't impact too much your code unless you've implemented your own IContainerJob which use a generic now.

# [2.9.0](https://github.com/Belphemur/Job.Scheduler/compare/v2.8.0...v2.9.0) (2022-07-13)


### Features

* **Queue:** Add registration of queue part of ASP.NET JobScheduler configuration ([eeb07fb](https://github.com/Belphemur/Job.Scheduler/commit/eeb07fb794cf6b6dabf5246e014dd5b200f835ce))
* **Queue:** Add support for queue. It's possible to register queues with their own max concurrency. ([1f0ae91](https://github.com/Belphemur/Job.Scheduler/commit/1f0ae91c868606e3931d43c85db33efa04efd6a5))

# [2.8.0](https://github.com/Belphemur/Job.Scheduler/compare/v2.7.3...v2.8.0) (2022-07-12)


### Features

* **ExponentialDecorrelatedJittedBackoffRetry:** Add a new retry strategy that is better than the normal exponential retry ([4c0277e](https://github.com/Belphemur/Job.Scheduler/commit/4c0277ecbda7fa3ef37070568cba4e10bb7ec405))

## [2.7.3](https://github.com/Belphemur/Job.Scheduler/compare/v2.7.2...v2.7.3) (2022-05-09)


### Bug Fixes

* **Docs:** Finally found the way to have the source code in symbol package ([8b00d50](https://github.com/Belphemur/Job.Scheduler/commit/8b00d50c767496f4cb710a23beedfcc0ad1b8039))

## [2.7.2](https://github.com/Belphemur/Job.Scheduler/compare/v2.7.1...v2.7.2) (2022-05-09)


### Bug Fixes

* **Docs:** Include source code in the symbol package ([f44c536](https://github.com/Belphemur/Job.Scheduler/commit/f44c536fb2577eae446a64bbf6c3b52cf6ca94d8))

## [2.7.1](https://github.com/Belphemur/Job.Scheduler/compare/v2.7.0...v2.7.1) (2022-04-17)


### Bug Fixes

* **Asp.Net Core:** Add missing documentation ([8d62525](https://github.com/Belphemur/Job.Scheduler/commit/8d6252524a90e26f1954c99b4eac3e05a6504ca6))

# [2.7.0](https://github.com/Belphemur/Job.Scheduler/compare/v2.6.0...v2.7.0) (2022-04-16)


### Bug Fixes

* **Docs:** Fix documentation ([b2cc37f](https://github.com/Belphemur/Job.Scheduler/commit/b2cc37f8952d47eb15a3774789c0b43455d102fe))


### Features

* **ServiceCollection:** Add easy method to add Job as scoped ([62e4a5e](https://github.com/Belphemur/Job.Scheduler/commit/62e4a5e4e0a9c881f17417b6021649d34dd96abf))

# [2.6.0](https://github.com/Belphemur/Job.Scheduler/compare/v2.5.1...v2.6.0) (2022-04-16)


### Features

* **Asp.net Core:** Add support for ASP.NET Core ([a4a1ce5](https://github.com/Belphemur/Job.Scheduler/commit/a4a1ce57df87610c9ea57bac9e6fd64e1e02fd67))
* **ContainerJob:** Add concept of Container job for advance usage ([e664657](https://github.com/Belphemur/Job.Scheduler/commit/e664657f78227ac1ea56112d3f81adca892cc431))
* **IDisposableAsync:** Add support for Async Disposable job ([0ce73fe](https://github.com/Belphemur/Job.Scheduler/commit/0ce73fe9ca3e923031b4248acbf3e3fc51a588d1))

## [2.5.1](https://github.com/Belphemur/Job.Scheduler/compare/v2.5.0...v2.5.1) (2022-04-16)


### Bug Fixes

* **Debounce:** Make debounce wait for the inner job ([687aca7](https://github.com/Belphemur/Job.Scheduler/commit/687aca7b0078a997908c57a14648ef6a8e4810bd))

# [2.5.0](https://github.com/Belphemur/Job.Scheduler/compare/v2.4.1...v2.5.0) (2021-10-05)


### Bug Fixes

* **Debounce:** Job already removed when second debouncing job comes ([1fcf630](https://github.com/Belphemur/Job.Scheduler/commit/1fcf630dce12e2c451078a52fe483dba5f1f4c6a))


### Features

* **DebounceJob:** Add new feature to implement debouncing jobs ([7976d03](https://github.com/Belphemur/Job.Scheduler/commit/7976d038da7018026fa9dae27d10e84d623f3a70))

## [2.4.1](https://github.com/Belphemur/Job.Scheduler/compare/v2.4.0...v2.4.1) (2021-09-28)


### Bug Fixes

* **TaskScheduler:** Be sure the task are run in proper thread depending if a TaskScheduler was given or not. ([d3d0aea](https://github.com/Belphemur/Job.Scheduler/commit/d3d0aea9224e4b150b567a7cfb2288e679e8a2cd))
* **Tests:** Wait for the right task ([99cd679](https://github.com/Belphemur/Job.Scheduler/commit/99cd679f87726515ecd1e3a7148859cc010c10ca))

# [2.4.0](https://github.com/Belphemur/Job.Scheduler/compare/v2.3.0...v2.4.0) (2021-09-27)


### Features

* **TaskScheduler:** Add option to run Job on a specific TaskScheduler ([812c396](https://github.com/Belphemur/Job.Scheduler/commit/812c396a0803c21fa3e262cc1d646dcbe8cb3d86))

# [2.3.0](https://github.com/Belphemur/Job.Scheduler/compare/v2.2.2...v2.3.0) (2021-07-27)


### Features

* **BackoffRetry:** Add 2 different backoff retry using the new delay between retries ([0484803](https://github.com/Belphemur/Job.Scheduler/commit/0484803cf58a83cf046c5c8f135f56ea5785ec0b))
* **Retry:** Let the user be able to define their own delay strategy between retries instead of a hardcoded value. ([c26802a](https://github.com/Belphemur/Job.Scheduler/commit/c26802a4b522df46259885060ec25345c721867e))

## [2.2.2](https://github.com/Belphemur/Job.Scheduler/compare/v2.2.1...v2.2.2) (2021-07-15)


### Bug Fixes

* **Disposing:** Dispose of task when possible ([1f31de5](https://github.com/Belphemur/Job.Scheduler/commit/1f31de5346903de90a366dbf182fc68f9c45bbd7))

## [2.2.1](https://github.com/Belphemur/Job.Scheduler/compare/v2.2.0...v2.2.1) (2021-05-22)


### Bug Fixes

* **Deadlock:** deadlock of task when job end ([b538f8d](https://github.com/Belphemur/Job.Scheduler/commit/b538f8d542e23b3a535e125125ed52bb5822efc7))

# [2.2.0](https://github.com/Belphemur/Job.Scheduler/compare/v2.1.1...v2.2.0) (2021-05-15)


### Features

* **AlwaysRetry:** Add configurable delay for always retry between each retry of the job ([4d6dde2](https://github.com/Belphemur/Job.Scheduler/commit/4d6dde2f8faacdada5318a42dd482b49dfd6eb7b))

## [2.1.1](https://github.com/Belphemur/Job.Scheduler/compare/v2.1.0...v2.1.1) (2021-04-13)


### Bug Fixes

* **Symbols:** Have symbols uploaded ([42b3dd5](https://github.com/Belphemur/Job.Scheduler/commit/42b3dd551ee6085239ba6c1ee45954187c5fc087))

# [2.1.0](https://github.com/Belphemur/Job.Scheduler/compare/v2.0.2...v2.1.0) (2021-04-08)


### Features

* **OpenTelemetry:** Add support for Open Telemetry ([16766dc](https://github.com/Belphemur/Job.Scheduler/commit/16766dc6749128718a37012b49ffb2a9d5e87beb))

## [2.0.2](https://github.com/Belphemur/Job.Scheduler/compare/v2.0.1...v2.0.2) (2021-03-22)


### Bug Fixes

* **ci:** Add semantic release NPM ([436871a](https://github.com/Belphemur/Job.Scheduler/commit/436871aeca53b30a32712219d534366e09c4b1d2))
* **Release:** Fix release script ([efa65cd](https://github.com/Belphemur/Job.Scheduler/commit/efa65cd86a035267021744535360afd968324d74))
