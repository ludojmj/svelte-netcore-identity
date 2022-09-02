<script>
  import "./variables.scss";
  import { Router, Route } from "svelte-navigator";
  import {
    OidcContext,
    authError,
    accessToken,
    idToken,
    isAuthenticated,
    isLoading,
    userInfo,
  } from "./oidc/components.module"; // "@dopry/svelte-oidc";
  import Header from "./lib/Header.svelte";
  import CrudManager from "./lib/CrudManager.svelte";
  import StuffCreate from "./lib/StuffCreate.svelte";
  import StuffRead from "./lib/StuffRead.svelte";
  import StuffUpdate from "./lib/StuffUpdate.svelte";
  import StuffDelete from "./lib/StuffDelete.svelte";
  import Error from "./lib/Error.svelte";

  let oidcConf = {
    issuer: "https://demo.duendesoftware.com",
    client_id: "interactive.public",
    redirect_uri: import.meta.env.VITE_REDIRECT_URI,
    post_logout_redirect_uri: import.meta.env.VITE_REDIRECT_URI
  };
  const displayOidcConf = false;
</script>

<main>
  <div class="container">
    <OidcContext
      issuer={oidcConf.issuer}
      client_id={oidcConf.client_id}
      redirect_uri={oidcConf.redirect_uri}
      post_logout_redirect_uri={oidcConf.post_logout_redirect_uri}
    >
      <Header />
      {#if $authError}
        <div class="alert alert-warning" role="alert">
          You need to login to access this site.
        </div>
      {/if}
      {#if $isAuthenticated}
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

      {#if displayOidcConf}
        <table class="table mt-5">
          <thead>
            <tr>
              <th scope="col">store</th>
              <th scope="col">value</th>
            </tr>
          </thead>
          <tbody>
            <tr>
              <th scope="row">isLoading</th>
              <td>{$isLoading}</td>
            </tr>
            <tr>
              <th scope="row">isAuthenticated</th>
              <td>{$isAuthenticated}</td>
            </tr>
            <tr>
              <th scope="row">accessToken</th>
              <td>
                {$accessToken}
              </td>
            </tr>
            <tr>
              <th scope="row">idToken</th>
              <td>{$idToken}</td>
            </tr>
            <tr>
              <th scope="row">userInfo</th>
              <td>
                <pre>{JSON.stringify($userInfo, null, 2) || ""}</pre>
              </td>
            </tr>
            <tr>
              <th scope="row">authError</th>
              <td>{$authError}</td>
            </tr>
          </tbody>
        </table>
      {/if}
    </OidcContext>
  </div>
</main>
