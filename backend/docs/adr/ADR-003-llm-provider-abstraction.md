# ADR-003: LLM Provider Abstraction

**Status:** Accepted

Application code calls `ILLMGateway`; provider-specific HTTP/SDK behavior is implemented by `ILLMProvider` adapters. This enables deterministic fakes, bounded retries, usage/cost tracking, and future Azure OpenAI support without coupling workflows to one SDK.
