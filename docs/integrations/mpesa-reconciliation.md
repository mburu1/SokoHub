# M-Pesa Reconciliation

1. Persist CheckoutRequestID before STK Push.
2. Callback → fast 200 → queue.
3. If no terminal state after ~90s → STK Query.
4. Daily job: local ledger vs Daraja statement.
