# Dev JWT signing keys

`jwt-private.dev.pem` / `jwt-public.dev.pem` are a throwaway RSA-2048 keypair generated
for local development only. They are gitignored (`**/keys/*.pem`) and must never be used
outside a developer's own machine.

For staging/production, generate a real keypair and mount it as a secret (Key Vault /
Secrets Manager / GCP Secret Manager) — do not commit keys to source control, and do not
reuse the dev keypair anywhere reachable from the internet.

Regenerate the dev pair anytime with:

```
openssl genrsa -out jwt-private.dev.pem 2048
openssl rsa -in jwt-private.dev.pem -pubout -out jwt-public.dev.pem
```
