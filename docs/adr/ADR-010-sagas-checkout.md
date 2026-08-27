# ADR-010: Sagas for multi-vendor checkout

## Status
Accepted

## Context
One customer order spans multiple vendors; inventory + payment + sub-orders must stay consistent.

## Decision
Use process managers (sagas) with compensating transactions and Outbox.
