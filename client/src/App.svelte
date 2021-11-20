<script>
  // App.svelte
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
  import Header from "./components/Header.svelte";
  import CrudManager from "./components/CrudManager.svelte";
  import StuffCreate from "./components/StuffCreate.svelte";
  import StuffRead from "./components/StuffRead.svelte";
  import StuffUpdate from "./components/StuffUpdate.svelte";
  import StuffDelete from "./components/StuffDelete.svelte";
  let oidcConf = {
    issuer: "https://demo.identityserver.io",
    client_id: "interactive.public",
    redirect_uri: "http://localhost:3000",
    post_logout_redirect_uri: "http://localhost:3000/",
  };
  if (isProd) {
    oidcConf.redirect_uri = "https://localhost:5001";
    oidcConf.post_logout_redirect_uri = "https://localhost:5001/";
  }
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
          <Route path="create">
            <StuffCreate />
          </Route>
          <!-- <Route path="read/:id" component={StuffRead} /> -->
          <Route path="read/:id" let:params>
            <StuffRead id={params.id} />
          </Route>
          <Route path="update/:id" let:params>
            <StuffUpdate id={params.id} />
          </Route>
          <Route path="delete/:id" let:params>
            <StuffDelete id={params.id} />
          </Route>
          <Route path="/">
            <CrudManager />
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

<style global lang="scss">
  table {
    table-layout: fixed;
  }

  td {
    white-space: normal !important;
    word-wrap: break-word;
    overflow: hidden;
  }

  textarea {
    overflow: hidden;
    resize: none;
  }

  @media screen and (max-width: 576px) {
    .table thead {
      display: none;
      border: none;
    }
    .table tr {
      display: block;
    }
    .table td {
      display: block;
      text-align: right;
      border: none;
    }
    .table td:before {
      content: attr(data-label);
      float: left !important;
      font-weight: bold;
    }
  }
</style>
