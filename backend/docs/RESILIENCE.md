# Resilience

External AI calls use a bounded timeout and a maximum of three total attempts in the LLM gateway. Retries are limited to transient transport/time-out failures; validation, authentication, malformed requests, and permanent provider errors are not retried. Backoff is capped and cancellation is propagated.

Future provider adapters should classify HTTP 408, 429, and 5xx responses explicitly, honor `Retry-After`, and add jitter. Circuit breakers and concurrency limits should be added only after dependency metrics justify them. A failed workflow becomes `Failed` with a correlation ID and safe error summary; cancellation becomes `Cancelled`.
