# Daraja Callback Security

- HTTPS only
- IP allowlist (Safaricom ranges)
- Idempotency on CheckoutRequestID + MpesaReceiptNumber
- Never run heavy work in the HTTP thread
