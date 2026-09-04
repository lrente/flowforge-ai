# AI Cost Tracking

`ITokenCostCalculator` calculates estimated cost from provider/model pricing supplied in configuration. Pricing is expressed per million input and output tokens. Unknown pricing returns zero and must be visible as `unpriced`, not confused with free usage.

Every LLM request should persist or emit provider, model, workflow, tenant/user scope, input tokens, output tokens, total tokens, latency, status, and estimated cost. Reports aggregate by workflow, user, tenant, provider, model, and date. Prompt/response payloads remain opt-in and are excluded by default for privacy.
