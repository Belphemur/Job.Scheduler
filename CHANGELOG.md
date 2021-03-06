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
