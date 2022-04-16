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
