# Embed Token Renewal (App Owns Data)

ContentView renews embed tokens without reloading the embedded content. It requests a new token five minutes before expiration; for tokens with less than ten minutes remaining, it renews halfway through the remaining lifetime. It also checks expiration when a background tab becomes visible or the page returns from the browser's back-forward cache.

## Security

The module-scoped `POST EmbedToken/Renew` WebAPI endpoint requires DNN module authorization and an antiforgery token. It rechecks workspace and resource permissions, resolves the RLS identity from server-side module settings, and uses the current user's roles. Custom RLS extension failures prevent token generation. Responses contain only `token` and UTC `expiration`, with HTTP caching disabled. Service principal credentials remain on the server.

## Failure Handling

Temporary failures are retried after 10 seconds with exponential backoff capped at 60 seconds. Each HTTP request has a 30-second timeout. HTTP 401/403 stops renewal; the user must restore their session or permissions and reload the page.

## Token Lifetime

The backend stops caching Microsoft Entra credentials ten minutes before their expiration and uses an isolated ADAL token cache when obtaining new credentials. This avoids issuing embed tokens with only a few minutes of remaining validity. This does not increase Power BI's maximum token lifetime; `lifetimeInMinutes` can only shorten it.

## Deployment

Deploy both the rebuilt module DLL and `scripts/ContentView.js`, then reload existing pages to initialize renewal. No database migration or new module setting is required. Verify in browser Network tools that `EmbedToken/Renew` returns a new token and expiration while the report retains its current filters and page.

## Testing

Run the focused client tests with Node.js 18 or later from the repository root:

```powershell
node --test tests/ContentView.TokenRenewal.test.js
```

Alternatively, run `npm test` from `src/DotNetNuke.PowerBI`. The tests simulate the SDK, network responses, and browser timers; they do not replace verification against a deployed DNN site and Power BI.

## References

See Microsoft's [token renewal guidance](https://learn.microsoft.com/en-us/javascript/api/overview/powerbi/refresh-token) for the distinction between embed-token renewal and `accessTokenProvider`, which is only supported for User Owns Data.

Return to the [documentation index](README.md).