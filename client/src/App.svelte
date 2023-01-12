<script>
  import "./variables.scss";
  import { VanillaOidc } from "@axa-fr/vanilla-oidc";
  import { Router, Route } from "svelte-navigator";
  import { tokens } from "./store.js";
  import { configuration } from "./oidcConf.js";
  import Header from "./lib/Header.svelte";
  import CrudManager from "./lib/CrudManager.svelte";
  import StuffCreate from "./lib/StuffCreate.svelte";
  import StuffRead from "./lib/StuffRead.svelte";
  import StuffUpdate from "./lib/StuffUpdate.svelte";
  import StuffDelete from "./lib/StuffDelete.svelte";
  import Error from "./lib/Error.svelte";

  const href = window.location.href;
  const vanillaOidc = VanillaOidc.getOrCreate(configuration);

  vanillaOidc.tryKeepExistingSessionAsync().then(() => {
    if (href.includes(configuration.redirect_uri)) {
      vanillaOidc.loginCallbackAsync().then(() => {
        window.location.href = "/";
      });
      return;
    }

    tokens.update(() => vanillaOidc.tokens);
  });
</script>

<main>
  <div class="container">
    <Header />
    {#if !$tokens}
      <div class="alert alert-warning" role="alert">
        You need to login to access this site.
      </div>
    {:else}
      <Router primary={false}>
        <Route path="/create">
          <StuffCreate />
        </Route>
        <Route path="/read/:id" let:params>
          <StuffRead id={params.id} />
        </Route>
        <Route path="/update/:id" let:params>
          <StuffUpdate id={params.id} />
        </Route>
        <Route path="/delete/:id" let:params>
          <StuffDelete id={params.id} />
        </Route>
        <Route path="/">
          <CrudManager />
        </Route>
        <Route>
          <Error msgErr="I got lost." />
        </Route>
      </Router>
    {/if}
  </div>
</main>
