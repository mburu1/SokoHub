<#
.SYNOPSIS
  SokoHub — create remaining critical folders & placeholder files (Windows).

.DESCRIPTION
  Run from the SokoHub repo root:
    cd "D:\Mwangi Wa Mburu\Coding\SokoHub"
    Set-ExecutionPolicy -Scope Process -ExecutionPolicy Bypass
    .\Add-SokoHubCriticalFolders.ps1

  Idempotent: skips existing paths. Creates ★ gaps only.

.NOTES
  Date: 2026-08-27
#>

$ErrorActionPreference = "Stop"
$Root = Get-Location

function Ensure-Dir([string]$Relative) {
    $path = Join-Path $Root $Relative
    if (-not (Test-Path $path)) {
        New-Item -ItemType Directory -Path $path -Force | Out-Null
        Write-Host "  + DIR  $Relative" -ForegroundColor Green
    } else {
        Write-Host "  = DIR  $Relative" -ForegroundColor DarkGray
    }
}

function Ensure-File([string]$Relative, [string]$Content = "") {
    $path = Join-Path $Root $Relative
    $dir = Split-Path $path -Parent
    if (-not (Test-Path $dir)) {
        New-Item -ItemType Directory -Path $dir -Force | Out-Null
    }
    if (-not (Test-Path $path)) {
        Set-Content -Path $path -Value $Content -Encoding UTF8
        Write-Host "  + FILE $Relative" -ForegroundColor Cyan
    } else {
        Write-Host "  = FILE $Relative" -ForegroundColor DarkGray
    }
}

Write-Host "`n=== SokoHub critical gap scaffold ===" -ForegroundColor Yellow
Write-Host "Root: $Root`n"

# ---------------------------------------------------------------------------
# Application — Sagas
# ---------------------------------------------------------------------------
Write-Host "[Application Sagas]" -ForegroundColor Magenta
Ensure-Dir "backend\src\SokoHub.Application\Common\Sagas"
Ensure-Dir "backend\src\SokoHub.Application\Common\FeatureFlags"
Ensure-Dir "backend\src\SokoHub.Application\Modules\Checkout\Sagas"
Ensure-Dir "backend\src\SokoHub.Application\Modules\Orders\Sagas"
Ensure-Dir "backend\src\SokoHub.Application\Modules\Payments\Sagas"

Ensure-File "backend\src\SokoHub.Application\Common\Sagas\ISaga.cs" @"
namespace SokoHub.Application.Common.Sagas;

/// <summary>Marker for long-running process managers (checkout, payout, refund).</summary>
public interface ISaga
{
    Guid CorrelationId { get; }
}
"@

Ensure-File "backend\src\SokoHub.Application\Modules\Checkout\Sagas\CheckoutSaga.cs" @"
namespace SokoHub.Application.Modules.Checkout.Sagas;

/// <summary>
/// Coordinates: inventory reservation → payment initiation → order creation / split.
/// Compensating actions on failure (release stock, cancel pending payment).
/// </summary>
public sealed class CheckoutSaga : SokoHub.Application.Common.Sagas.ISaga
{
    public Guid CorrelationId { get; init; }
    // TODO: state machine + handlers
}
"@

Ensure-File "backend\src\SokoHub.Application\Modules\Orders\Sagas\OrderSplitSaga.cs" @"
namespace SokoHub.Application.Modules.Orders.Sagas;

/// <summary>One cart → N vendor sub-orders.</summary>
public sealed class OrderSplitSaga : SokoHub.Application.Common.Sagas.ISaga
{
    public Guid CorrelationId { get; init; }
}
"@

Ensure-File "backend\src\SokoHub.Application\Modules\Payments\Sagas\PaymentSaga.cs" @"
namespace SokoHub.Application.Modules.Payments.Sagas;

public sealed class PaymentSaga : SokoHub.Application.Common.Sagas.ISaga
{
    public Guid CorrelationId { get; init; }
}
"@

Ensure-File "backend\src\SokoHub.Application\Modules\Payments\Sagas\RefundSaga.cs" @"
namespace SokoHub.Application.Modules.Payments.Sagas;

public sealed class RefundSaga : SokoHub.Application.Common.Sagas.ISaga
{
    public Guid CorrelationId { get; init; }
}
"@

Ensure-File "backend\src\SokoHub.Application\Modules\Payments\Sagas\PayoutSaga.cs" @"
namespace SokoHub.Application.Modules.Payments.Sagas;

public sealed class PayoutSaga : SokoHub.Application.Common.Sagas.ISaga
{
    public Guid CorrelationId { get; init; }
}
"@

Ensure-File "backend\src\SokoHub.Application\Common\FeatureFlags\FeatureFlagKeys.cs" @"
namespace SokoHub.Application.Common.FeatureFlags;

public static class FeatureFlagKeys
{
    public const string MpesaStkPush = ""MpesaStkPush"";
    public const string CardPayments = ""CardPayments"";
    public const string VendorPayouts = ""VendorPayouts"";
}
"@

# ---------------------------------------------------------------------------
# Infrastructure — StkQuery, OpenTelemetry, FeatureManagement, OpenSearch
# ---------------------------------------------------------------------------
Write-Host "`n[Infrastructure]" -ForegroundColor Magenta
Ensure-Dir "backend\src\SokoHub.Infrastructure\Payments\Mpesa\StkQuery"
Ensure-Dir "backend\src\SokoHub.Infrastructure\Observability\OpenTelemetry"
Ensure-Dir "backend\src\SokoHub.Infrastructure\FeatureManagement"
Ensure-Dir "backend\src\SokoHub.Infrastructure\Localization"
Ensure-Dir "backend\src\SokoHub.Infrastructure\Search\OpenSearch"
Ensure-Dir "backend\src\SokoHub.Infrastructure\BackgroundJobs\Payouts"

Ensure-File "backend\src\SokoHub.Infrastructure\Payments\Mpesa\StkQuery\StkQueryService.cs" @"
namespace SokoHub.Infrastructure.Payments.Mpesa.StkQuery;

/// <summary>
/// Calls Daraja STK Push Query when callback is missing (~90s).
/// Source of truth for terminal state when webhook is silent.
/// </summary>
public sealed class StkQueryService
{
    // TODO: inject DarajaClient, persist result, publish domain event
}
"@

Ensure-File "backend\src\SokoHub.Infrastructure\Payments\Mpesa\StkQuery\StkQueryRequestBuilder.cs" @"
namespace SokoHub.Infrastructure.Payments.Mpesa.StkQuery;

public static class StkQueryRequestBuilder
{
    // TODO: BusinessShortCode, Password, Timestamp, CheckoutRequestID
}
"@

Ensure-File "backend\src\SokoHub.Infrastructure\Observability\OpenTelemetry\OpenTelemetryConfiguration.cs" @"
namespace SokoHub.Infrastructure.Observability.OpenTelemetry;

/// <summary>Wire OTLP traces, metrics, logs via ServiceDefaults.</summary>
public static class OpenTelemetryConfiguration
{
    // TODO: AddOpenTelemetry().WithTracing().WithMetrics()
}
"@

Ensure-File "backend\src\SokoHub.Infrastructure\FeatureManagement\FeatureManagementExtensions.cs" @"
namespace SokoHub.Infrastructure.FeatureManagement;

public static class FeatureManagementExtensions
{
    // TODO: services.AddFeatureManagement(configuration)
}
"@

Ensure-File "backend\src\SokoHub.Infrastructure\Search\OpenSearch\OpenSearchClient.cs" @"
namespace SokoHub.Infrastructure.Search.OpenSearch;

public sealed class OpenSearchClient
{
    // TODO: product index, facet queries
}
"@

# ---------------------------------------------------------------------------
# API — Compliance + Localization middleware
# ---------------------------------------------------------------------------
Write-Host "`n[Api]" -ForegroundColor Magenta
Ensure-Dir "backend\src\SokoHub.Api\Controllers\Compliance"
Ensure-Dir "backend\src\SokoHub.Api\Middleware\Localization"

Ensure-File "backend\src\SokoHub.Api\Controllers\Compliance\ComplianceController.cs" @"
using Microsoft.AspNetCore.Mvc;

namespace SokoHub.Api.Controllers.Compliance;

[ApiController]
[Route(""api/v1/compliance"")]
public sealed class ComplianceController : ControllerBase
{
    /// <summary>Kenya Data Protection Act — consent / subject-request stubs.</summary>
    [HttpGet(""health"")]
    public IActionResult Health() => Ok(new { status = ""ok"" });
}
"@

Ensure-File "backend\src\SokoHub.Api\Middleware\Localization\LocalizationMiddleware.cs" @"
namespace SokoHub.Api.Middleware.Localization;

/// <summary>Sets culture from Accept-Language (en / sw).</summary>
public sealed class LocalizationMiddleware
{
    // TODO: RequestCultureProvider
}
"@

Ensure-Dir "backend\src\SokoHub.Api\Middleware\Security"
Ensure-File "backend\src\SokoHub.Api\Middleware\Security\DarajaIpAllowlistMiddleware.cs" @"
namespace SokoHub.Api.Middleware.Security;

/// <summary>
/// Restrict M-Pesa callback routes to Safaricom IP ranges (configurable).
/// Pair with fast 200 ACK + queue offload in WebhooksController.
/// </summary>
public sealed class DarajaIpAllowlistMiddleware
{
    // TODO: options + remote IP check
}
"@

# ---------------------------------------------------------------------------
# Domain — Compliance module stub
# ---------------------------------------------------------------------------
Write-Host "`n[Domain Compliance]" -ForegroundColor Magenta
Ensure-Dir "backend\src\SokoHub.Domain\Modules\Compliance"
Ensure-File "backend\src\SokoHub.Domain\Modules\Compliance\ConsentRecord.cs" @"
namespace SokoHub.Domain.Modules.Compliance;

public sealed class ConsentRecord
{
    public Guid Id { get; init; }
    public Guid SubjectId { get; init; }
    public string Purpose { get; init; } = string.Empty;
    public DateTimeOffset GrantedAt { get; init; }
}
"@

# ---------------------------------------------------------------------------
# Tests — ResilienceTests
# ---------------------------------------------------------------------------
Write-Host "`n[Tests]" -ForegroundColor Magenta
Ensure-Dir "backend\tests\SokoHub.ResilienceTests"
Ensure-File "backend\tests\SokoHub.ResilienceTests\SokoHub.ResilienceTests.csproj" @"
<Project Sdk=""Microsoft.NET.Sdk"">
  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
    <IsPackable>false</IsPackable>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include=""Microsoft.NET.Test.Sdk"" />
    <PackageReference Include=""xunit"" />
    <PackageReference Include=""xunit.runner.visualstudio"" />
  </ItemGroup>
</Project>
"@
Ensure-File "backend\tests\SokoHub.ResilienceTests\CircuitBreakerTests.cs" @"
namespace SokoHub.ResilienceTests;

public class CircuitBreakerTests
{
    // TODO: Polly / HTTP resilience behaviour
}
"@

# ---------------------------------------------------------------------------
# Frontend locales
# ---------------------------------------------------------------------------
Write-Host "`n[Frontend i18n]" -ForegroundColor Magenta
Ensure-Dir "frontend\src\locales"
Ensure-File "frontend\src\locales\en.json" "{`n  `"app.name`": `"SokoHub`",`n  `"nav.home`": `"Home`"`n}`n"
Ensure-File "frontend\src\locales\sw.json" "{`n  `"app.name`": `"SokoHub`",`n  `"nav.home`": `"Nyumbani`"`n}`n"

# ---------------------------------------------------------------------------
# Docs
# ---------------------------------------------------------------------------
Write-Host "`n[Docs]" -ForegroundColor Magenta
Ensure-Dir "docs\compliance"
Ensure-Dir "docs\architecture"
Ensure-Dir "docs\integrations"
Ensure-Dir "docs\security"
Ensure-Dir "docs\diagrams\sequence"
Ensure-Dir "docs\adr"

Ensure-File "docs\architecture\sagas-and-orchestration.md" @"
# Sagas and Orchestration

Checkout, order-split, payment, refund, and vendor payout process managers.
Compensate on failure. Prefer Outbox for reliable messaging.
"@

Ensure-File "docs\integrations\mpesa-reconciliation.md" @"
# M-Pesa Reconciliation

1. Persist CheckoutRequestID before STK Push.
2. Callback → fast 200 → queue.
3. If no terminal state after ~90s → STK Query.
4. Daily job: local ledger vs Daraja statement.
"@

Ensure-File "docs\security\daraja-callback-security.md" @"
# Daraja Callback Security

- HTTPS only
- IP allowlist (Safaricom ranges)
- Idempotency on CheckoutRequestID + MpesaReceiptNumber
- Never run heavy work in the HTTP thread
"@

Ensure-File "docs\compliance\kenya-data-protection-act.md" @"
# Kenya Data Protection Act

Consent, purpose limitation, access/erasure requests, PII classification, audit of access.
"@

Ensure-File "docs\diagrams\sequence\mpesa-reconciliation-flow.puml" @"
@startuml
title M-Pesa STK Reconciliation
participant Worker
participant StkQuery
participant Daraja
participant Ledger
Worker -> StkQuery: pending > 90s
StkQuery -> Daraja: stkpushquery
Daraja --> StkQuery: terminal status
StkQuery -> Ledger: settle / mismatch flag
@enduml
"@

Ensure-File "docs\diagrams\sequence\order-split-flow.puml" @"
@startuml
title Multi-vendor Order Split
participant Checkout
participant OrderSplitSaga
participant Orders
participant Inventory
Checkout -> OrderSplitSaga: cart confirmed
OrderSplitSaga -> Inventory: reserve per vendor
OrderSplitSaga -> Orders: create sub-orders
@enduml
"@

Ensure-File "docs\adr\ADR-010-sagas-checkout.md" @"
# ADR-010: Sagas for multi-vendor checkout

## Status
Accepted

## Context
One customer order spans multiple vendors; inventory + payment + sub-orders must stay consistent.

## Decision
Use process managers (sagas) with compensating transactions and Outbox.
"@

Ensure-File "docs\adr\ADR-011-search-engine.md" @"
# ADR-011: Search engine

## Status
Accepted

## Decision
OpenSearch (or Meilisearch for local) for faceted catalog search; Redis as result cache only.
"@

Ensure-File "docs\adr\ADR-012-localization.md" @"
# ADR-012: Localization

## Status
Accepted

## Decision
English + Kiswahili. Culture from Accept-Language. Money VO for KES/USD display.
"@

Write-Host "`n=== Done. Review git status and commit. ===" -ForegroundColor Yellow
Write-Host "Suggested:" -ForegroundColor White
Write-Host '  git add -A'
Write-Host '  git commit -m "chore: scaffold critical gaps (sagas, stkquery, otel, compliance, docs)"'
Write-Host ""
