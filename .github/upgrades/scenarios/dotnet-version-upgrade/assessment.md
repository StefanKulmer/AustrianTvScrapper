# Projects and dependencies analysis

This document provides a comprehensive overview of the projects and their dependencies in the context of upgrading to .NETCoreApp,Version=v10.0.

## Table of Contents

- [Executive Summary](#executive-Summary)
  - [Highlevel Metrics](#highlevel-metrics)
  - [Projects Compatibility](#projects-compatibility)
  - [Package Compatibility](#package-compatibility)
  - [API Compatibility](#api-compatibility)
  - [Binding Redirect Configuration](#binding-redirect-configuration)
- [Aggregate NuGet packages details](#aggregate-nuget-packages-details)
- [Top API Migration Challenges](#top-api-migration-challenges)
  - [Technologies and Features](#technologies-and-features)
  - [Most Frequent API Issues](#most-frequent-api-issues)
- [Projects Relationship Graph](#projects-relationship-graph)
- [Project Details](#project-details)

  - [src\Downloader\Downloader.csproj](#srcdownloaderdownloadercsproj)
  - [src\DownloadListCreator\DownloadListCreator.csproj](#srcdownloadlistcreatordownloadlistcreatorcsproj)
  - [src\OrfDataProvider\OrfDataProvider.csproj](#srcorfdataproviderorfdataprovidercsproj)
  - [src\Services\Services.csproj](#srcservicesservicescsproj)
  - [src\StartUp\StartUp.csproj](#srcstartupstartupcsproj)
  - [src\Subscription\Subscription.csproj](#srcsubscriptionsubscriptioncsproj)
  - [src\tests\Services.Tests\Services.Tests.csproj](#srctestsservicestestsservicestestscsproj)


## Executive Summary

### Highlevel Metrics

| Metric | Count | Status |
| :--- | :---: | :--- |
| Total Projects | 7 | 0 require upgrade |
| Total NuGet Packages | 17 | All compatible |
| Total Code Files | 82 |  |
| Total Code Files with Incidents | 0 |  |
| Total Lines of Code | 3534 |  |
| Total Number of Issues | 0 |  |
| Estimated LOC to modify | 0+ | at least 0,0% of codebase |

### Projects Compatibility

| Project | Target Framework | Difficulty | Package Issues | API Issues | Binding Issues | Est. LOC Impact | Description |
| :--- | :---: | :---: | :---: | :---: | :---: | :---: | :--- |
| [src\Downloader\Downloader.csproj](#srcdownloaderdownloadercsproj) | net10.0 | ✅ None | 0 | 0 | 0 |  | ClassLibrary, Sdk Style = True |
| [src\DownloadListCreator\DownloadListCreator.csproj](#srcdownloadlistcreatordownloadlistcreatorcsproj) | net10.0 | ✅ None | 0 | 0 | 0 |  | ClassLibrary, Sdk Style = True |
| [src\OrfDataProvider\OrfDataProvider.csproj](#srcorfdataproviderorfdataprovidercsproj) | net10.0 | ✅ None | 0 | 0 | 0 |  | ClassLibrary, Sdk Style = True |
| [src\Services\Services.csproj](#srcservicesservicescsproj) | net10.0 | ✅ None | 0 | 0 | 0 |  | ClassLibrary, Sdk Style = True |
| [src\StartUp\StartUp.csproj](#srcstartupstartupcsproj) | net10.0 | ✅ None | 0 | 0 | 0 |  | DotNetCoreApp, Sdk Style = True |
| [src\Subscription\Subscription.csproj](#srcsubscriptionsubscriptioncsproj) | net10.0 | ✅ None | 0 | 0 | 0 |  | ClassLibrary, Sdk Style = True |
| [src\tests\Services.Tests\Services.Tests.csproj](#srctestsservicestestsservicestestscsproj) | net10.0 | ✅ None | 0 | 0 | 0 |  | DotNetCoreApp, Sdk Style = True |

### Package Compatibility

| Status | Count | Percentage |
| :--- | :---: | :---: |
| ✅ Compatible | 17 | 100,0% |
| ⚠️ Incompatible | 0 | 0,0% |
| 🔄 Upgrade Recommended | 0 | 0,0% |
| ***Total NuGet Packages*** | ***17*** | ***100%*** |

### API Compatibility

| Category | Count | Impact |
| :--- | :---: | :--- |
| 🔴 Binary Incompatible | 0 | High - Require code changes |
| 🟡 Source Incompatible | 0 | Medium - Needs re-compilation and potential conflicting API error fixing |
| 🔵 Behavioral change | 0 | Low - Behavioral changes that may require testing at runtime |
| ✅ Compatible | 0 |  |
| ***Total APIs Analyzed*** | ***0*** |  |

## Aggregate NuGet packages details

| Package | Current Version | Suggested Version | Projects | Description |
| :--- | :---: | :---: | :--- | :--- |
| coverlet.collector | 10.0.1 |  | [Services.Tests.csproj](#srctestsservicestestsservicestestscsproj) | ✅Compatible |
| HtmlAgilityPack | 1.13.0 |  | [Services.csproj](#srcservicesservicescsproj)<br/>[Services.Tests.csproj](#srctestsservicestestsservicestestscsproj) | ✅Compatible |
| Microsoft.Extensions.Configuration | 9.0.0-preview.5.24306.7 |  | [StartUp.csproj](#srcstartupstartupcsproj) | ✅Compatible |
| Microsoft.Extensions.Configuration.Abstractions | 9.0.0-preview.5.24306.7 |  | [StartUp.csproj](#srcstartupstartupcsproj) | ✅Compatible |
| Microsoft.Extensions.Configuration.Binder | 9.0.0-preview.5.24306.7 |  | [StartUp.csproj](#srcstartupstartupcsproj) | ✅Compatible |
| Microsoft.Extensions.Configuration.Json | 9.0.0-preview.5.24306.7 |  | [StartUp.csproj](#srcstartupstartupcsproj) | ✅Compatible |
| Microsoft.Extensions.DependencyInjection | 9.0.0-preview.5.24306.7 |  | [StartUp.csproj](#srcstartupstartupcsproj) | ✅Compatible |
| Microsoft.Extensions.DependencyInjection.Abstractions | 10.0.12 |  | [Services.csproj](#srcservicesservicescsproj)<br/>[StartUp.csproj](#srcstartupstartupcsproj)<br/>[Subscription.csproj](#srcsubscriptionsubscriptioncsproj) | ✅Compatible |
| Microsoft.Extensions.Hosting.Abstractions | 8.0.0 |  | [StartUp.csproj](#srcstartupstartupcsproj) | ✅Compatible |
| Microsoft.Extensions.Options | 10.0.12 |  | [Downloader.csproj](#srcdownloaderdownloadercsproj)<br/>[DownloadListCreator.csproj](#srcdownloadlistcreatordownloadlistcreatorcsproj)<br/>[Subscription.csproj](#srcsubscriptionsubscriptioncsproj) | ✅Compatible |
| Microsoft.Extensions.Options.ConfigurationExtensions | 8.0.0 |  | [StartUp.csproj](#srcstartupstartupcsproj) | ✅Compatible |
| Microsoft.NET.Test.Sdk | 18.10.0 |  | [Services.Tests.csproj](#srctestsservicestestsservicestestscsproj) | ✅Compatible |
| System.CommandLine | 2.0.0-beta4.22272.1 |  | [StartUp.csproj](#srcstartupstartupcsproj) | ✅Compatible |
| System.CommandLine.NamingConventionBinder | 2.0.0-beta4.22272.1 |  | [StartUp.csproj](#srcstartupstartupcsproj) | ✅Compatible |
| System.IO.Abstractions | 22.2.0 |  | [Downloader.csproj](#srcdownloaderdownloadercsproj)<br/>[DownloadListCreator.csproj](#srcdownloadlistcreatordownloadlistcreatorcsproj)<br/>[Subscription.csproj](#srcsubscriptionsubscriptioncsproj) | ✅Compatible |
| xunit | 2.9.3 |  | [Services.Tests.csproj](#srctestsservicestestsservicestestscsproj) | ✅Compatible |
| xunit.runner.visualstudio | 4.0.0 |  | [Services.Tests.csproj](#srctestsservicestestsservicestestscsproj) | ✅Compatible |

## Top API Migration Challenges

### Technologies and Features

| Technology | Issues | Percentage | Migration Path |
| :--- | :---: | :---: | :--- |

### Most Frequent API Issues

| API | Count | Percentage | Category |
| :--- | :---: | :---: | :--- |

## Projects Relationship Graph

Legend:
📦 SDK-style project
⚙️ Classic project

```mermaid
flowchart LR
    P1["<b>📦&nbsp;StartUp.csproj</b><br/><small>net10.0</small>"]
    P2["<b>📦&nbsp;Services.csproj</b><br/><small>net10.0</small>"]
    P3["<b>📦&nbsp;Services.Tests.csproj</b><br/><small>net10.0</small>"]
    P4["<b>📦&nbsp;Subscription.csproj</b><br/><small>net10.0</small>"]
    P5["<b>📦&nbsp;OrfDataProvider.csproj</b><br/><small>net10.0</small>"]
    P6["<b>📦&nbsp;Downloader.csproj</b><br/><small>net10.0</small>"]
    P7["<b>📦&nbsp;DownloadListCreator.csproj</b><br/><small>net10.0</small>"]
    P1 --> P7
    P1 --> P6
    P1 --> P2
    P1 --> P4
    P3 --> P2
    P6 --> P7
    P6 --> P5
    P7 --> P5
    P7 --> P4
    click P1 "#srcstartupstartupcsproj"
    click P2 "#srcservicesservicescsproj"
    click P3 "#srctestsservicestestsservicestestscsproj"
    click P4 "#srcsubscriptionsubscriptioncsproj"
    click P5 "#srcorfdataproviderorfdataprovidercsproj"
    click P6 "#srcdownloaderdownloadercsproj"
    click P7 "#srcdownloadlistcreatordownloadlistcreatorcsproj"

```

## Project Details

<a id="srcdownloaderdownloadercsproj"></a>
### src\Downloader\Downloader.csproj

#### Project Info

- **Current Target Framework:** net10.0✅
- **SDK-style**: True
- **Project Kind:** ClassLibrary
- **Dependencies**: 2
- **Dependants**: 1
- **Number of Files**: 5
- **Lines of Code**: 361
- **Estimated LOC to modify**: 0+ (at least 0,0% of the project)

#### Dependency Graph

Legend:
📦 SDK-style project
⚙️ Classic project

```mermaid
flowchart TB
    subgraph upstream["Dependants (1)"]
        P1["<b>📦&nbsp;StartUp.csproj</b><br/><small>net10.0</small>"]
        click P1 "#srcstartupstartupcsproj"
    end
    subgraph current["Downloader.csproj"]
        MAIN["<b>📦&nbsp;Downloader.csproj</b><br/><small>net10.0</small>"]
        click MAIN "#srcdownloaderdownloadercsproj"
    end
    subgraph downstream["Dependencies (2"]
        P7["<b>📦&nbsp;DownloadListCreator.csproj</b><br/><small>net10.0</small>"]
        P5["<b>📦&nbsp;OrfDataProvider.csproj</b><br/><small>net10.0</small>"]
        click P7 "#srcdownloadlistcreatordownloadlistcreatorcsproj"
        click P5 "#srcorfdataproviderorfdataprovidercsproj"
    end
    P1 --> MAIN
    MAIN --> P7
    MAIN --> P5

```

### API Compatibility

| Category | Count | Impact |
| :--- | :---: | :--- |
| 🔴 Binary Incompatible | 0 | High - Require code changes |
| 🟡 Source Incompatible | 0 | Medium - Needs re-compilation and potential conflicting API error fixing |
| 🔵 Behavioral change | 0 | Low - Behavioral changes that may require testing at runtime |
| ✅ Compatible | 0 |  |
| ***Total APIs Analyzed*** | ***0*** |  |

<a id="srcdownloadlistcreatordownloadlistcreatorcsproj"></a>
### src\DownloadListCreator\DownloadListCreator.csproj

#### Project Info

- **Current Target Framework:** net10.0✅
- **SDK-style**: True
- **Project Kind:** ClassLibrary
- **Dependencies**: 2
- **Dependants**: 2
- **Number of Files**: 6
- **Lines of Code**: 271
- **Estimated LOC to modify**: 0+ (at least 0,0% of the project)

#### Dependency Graph

Legend:
📦 SDK-style project
⚙️ Classic project

```mermaid
flowchart TB
    subgraph upstream["Dependants (2)"]
        P1["<b>📦&nbsp;StartUp.csproj</b><br/><small>net10.0</small>"]
        P6["<b>📦&nbsp;Downloader.csproj</b><br/><small>net10.0</small>"]
        click P1 "#srcstartupstartupcsproj"
        click P6 "#srcdownloaderdownloadercsproj"
    end
    subgraph current["DownloadListCreator.csproj"]
        MAIN["<b>📦&nbsp;DownloadListCreator.csproj</b><br/><small>net10.0</small>"]
        click MAIN "#srcdownloadlistcreatordownloadlistcreatorcsproj"
    end
    subgraph downstream["Dependencies (2"]
        P5["<b>📦&nbsp;OrfDataProvider.csproj</b><br/><small>net10.0</small>"]
        P4["<b>📦&nbsp;Subscription.csproj</b><br/><small>net10.0</small>"]
        click P5 "#srcorfdataproviderorfdataprovidercsproj"
        click P4 "#srcsubscriptionsubscriptioncsproj"
    end
    P1 --> MAIN
    P6 --> MAIN
    MAIN --> P5
    MAIN --> P4

```

### API Compatibility

| Category | Count | Impact |
| :--- | :---: | :--- |
| 🔴 Binary Incompatible | 0 | High - Require code changes |
| 🟡 Source Incompatible | 0 | Medium - Needs re-compilation and potential conflicting API error fixing |
| 🔵 Behavioral change | 0 | Low - Behavioral changes that may require testing at runtime |
| ✅ Compatible | 0 |  |
| ***Total APIs Analyzed*** | ***0*** |  |

<a id="srcorfdataproviderorfdataprovidercsproj"></a>
### src\OrfDataProvider\OrfDataProvider.csproj

#### Project Info

- **Current Target Framework:** net10.0✅
- **SDK-style**: True
- **Project Kind:** ClassLibrary
- **Dependencies**: 0
- **Dependants**: 2
- **Number of Files**: 9
- **Lines of Code**: 526
- **Estimated LOC to modify**: 0+ (at least 0,0% of the project)

#### Dependency Graph

Legend:
📦 SDK-style project
⚙️ Classic project

```mermaid
flowchart TB
    subgraph upstream["Dependants (2)"]
        P6["<b>📦&nbsp;Downloader.csproj</b><br/><small>net10.0</small>"]
        P7["<b>📦&nbsp;DownloadListCreator.csproj</b><br/><small>net10.0</small>"]
        click P6 "#srcdownloaderdownloadercsproj"
        click P7 "#srcdownloadlistcreatordownloadlistcreatorcsproj"
    end
    subgraph current["OrfDataProvider.csproj"]
        MAIN["<b>📦&nbsp;OrfDataProvider.csproj</b><br/><small>net10.0</small>"]
        click MAIN "#srcorfdataproviderorfdataprovidercsproj"
    end
    P6 --> MAIN
    P7 --> MAIN

```

### API Compatibility

| Category | Count | Impact |
| :--- | :---: | :--- |
| 🔴 Binary Incompatible | 0 | High - Require code changes |
| 🟡 Source Incompatible | 0 | Medium - Needs re-compilation and potential conflicting API error fixing |
| 🔵 Behavioral change | 0 | Low - Behavioral changes that may require testing at runtime |
| ✅ Compatible | 0 |  |
| ***Total APIs Analyzed*** | ***0*** |  |

<a id="srcservicesservicescsproj"></a>
### src\Services\Services.csproj

#### Project Info

- **Current Target Framework:** net10.0✅
- **SDK-style**: True
- **Project Kind:** ClassLibrary
- **Dependencies**: 0
- **Dependants**: 2
- **Number of Files**: 32
- **Lines of Code**: 753
- **Estimated LOC to modify**: 0+ (at least 0,0% of the project)

#### Dependency Graph

Legend:
📦 SDK-style project
⚙️ Classic project

```mermaid
flowchart TB
    subgraph upstream["Dependants (2)"]
        P1["<b>📦&nbsp;StartUp.csproj</b><br/><small>net10.0</small>"]
        P3["<b>📦&nbsp;Services.Tests.csproj</b><br/><small>net10.0</small>"]
        click P1 "#srcstartupstartupcsproj"
        click P3 "#srctestsservicestestsservicestestscsproj"
    end
    subgraph current["Services.csproj"]
        MAIN["<b>📦&nbsp;Services.csproj</b><br/><small>net10.0</small>"]
        click MAIN "#srcservicesservicescsproj"
    end
    P1 --> MAIN
    P3 --> MAIN

```

### API Compatibility

| Category | Count | Impact |
| :--- | :---: | :--- |
| 🔴 Binary Incompatible | 0 | High - Require code changes |
| 🟡 Source Incompatible | 0 | Medium - Needs re-compilation and potential conflicting API error fixing |
| 🔵 Behavioral change | 0 | Low - Behavioral changes that may require testing at runtime |
| ✅ Compatible | 0 |  |
| ***Total APIs Analyzed*** | ***0*** |  |

<a id="srcstartupstartupcsproj"></a>
### src\StartUp\StartUp.csproj

#### Project Info

- **Current Target Framework:** net10.0✅
- **SDK-style**: True
- **Project Kind:** DotNetCoreApp
- **Dependencies**: 4
- **Dependants**: 0
- **Number of Files**: 15
- **Lines of Code**: 1150
- **Estimated LOC to modify**: 0+ (at least 0,0% of the project)

#### Dependency Graph

Legend:
📦 SDK-style project
⚙️ Classic project

```mermaid
flowchart TB
    subgraph current["StartUp.csproj"]
        MAIN["<b>📦&nbsp;StartUp.csproj</b><br/><small>net10.0</small>"]
        click MAIN "#srcstartupstartupcsproj"
    end
    subgraph downstream["Dependencies (4"]
        P7["<b>📦&nbsp;DownloadListCreator.csproj</b><br/><small>net10.0</small>"]
        P6["<b>📦&nbsp;Downloader.csproj</b><br/><small>net10.0</small>"]
        P2["<b>📦&nbsp;Services.csproj</b><br/><small>net10.0</small>"]
        P4["<b>📦&nbsp;Subscription.csproj</b><br/><small>net10.0</small>"]
        click P7 "#srcdownloadlistcreatordownloadlistcreatorcsproj"
        click P6 "#srcdownloaderdownloadercsproj"
        click P2 "#srcservicesservicescsproj"
        click P4 "#srcsubscriptionsubscriptioncsproj"
    end
    MAIN --> P7
    MAIN --> P6
    MAIN --> P2
    MAIN --> P4

```

### API Compatibility

| Category | Count | Impact |
| :--- | :---: | :--- |
| 🔴 Binary Incompatible | 0 | High - Require code changes |
| 🟡 Source Incompatible | 0 | Medium - Needs re-compilation and potential conflicting API error fixing |
| 🔵 Behavioral change | 0 | Low - Behavioral changes that may require testing at runtime |
| ✅ Compatible | 0 |  |
| ***Total APIs Analyzed*** | ***0*** |  |

<a id="srcsubscriptionsubscriptioncsproj"></a>
### src\Subscription\Subscription.csproj

#### Project Info

- **Current Target Framework:** net10.0✅
- **SDK-style**: True
- **Project Kind:** ClassLibrary
- **Dependencies**: 0
- **Dependants**: 2
- **Number of Files**: 9
- **Lines of Code**: 223
- **Estimated LOC to modify**: 0+ (at least 0,0% of the project)

#### Dependency Graph

Legend:
📦 SDK-style project
⚙️ Classic project

```mermaid
flowchart TB
    subgraph upstream["Dependants (2)"]
        P1["<b>📦&nbsp;StartUp.csproj</b><br/><small>net10.0</small>"]
        P7["<b>📦&nbsp;DownloadListCreator.csproj</b><br/><small>net10.0</small>"]
        click P1 "#srcstartupstartupcsproj"
        click P7 "#srcdownloadlistcreatordownloadlistcreatorcsproj"
    end
    subgraph current["Subscription.csproj"]
        MAIN["<b>📦&nbsp;Subscription.csproj</b><br/><small>net10.0</small>"]
        click MAIN "#srcsubscriptionsubscriptioncsproj"
    end
    P1 --> MAIN
    P7 --> MAIN

```

### API Compatibility

| Category | Count | Impact |
| :--- | :---: | :--- |
| 🔴 Binary Incompatible | 0 | High - Require code changes |
| 🟡 Source Incompatible | 0 | Medium - Needs re-compilation and potential conflicting API error fixing |
| 🔵 Behavioral change | 0 | Low - Behavioral changes that may require testing at runtime |
| ✅ Compatible | 0 |  |
| ***Total APIs Analyzed*** | ***0*** |  |

<a id="srctestsservicestestsservicestestscsproj"></a>
### src\tests\Services.Tests\Services.Tests.csproj

#### Project Info

- **Current Target Framework:** net10.0✅
- **SDK-style**: True
- **Project Kind:** DotNetCoreApp
- **Dependencies**: 1
- **Dependants**: 0
- **Number of Files**: 9
- **Lines of Code**: 250
- **Estimated LOC to modify**: 0+ (at least 0,0% of the project)

#### Dependency Graph

Legend:
📦 SDK-style project
⚙️ Classic project

```mermaid
flowchart TB
    subgraph current["Services.Tests.csproj"]
        MAIN["<b>📦&nbsp;Services.Tests.csproj</b><br/><small>net10.0</small>"]
        click MAIN "#srctestsservicestestsservicestestscsproj"
    end
    subgraph downstream["Dependencies (1"]
        P2["<b>📦&nbsp;Services.csproj</b><br/><small>net10.0</small>"]
        click P2 "#srcservicesservicescsproj"
    end
    MAIN --> P2

```

### API Compatibility

| Category | Count | Impact |
| :--- | :---: | :--- |
| 🔴 Binary Incompatible | 0 | High - Require code changes |
| 🟡 Source Incompatible | 0 | Medium - Needs re-compilation and potential conflicting API error fixing |
| 🔵 Behavioral change | 0 | Low - Behavioral changes that may require testing at runtime |
| ✅ Compatible | 0 |  |
| ***Total APIs Analyzed*** | ***0*** |  |

