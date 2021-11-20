<script>
  // Header.svelte
  let logo = "/logo.svg";
  import {
    LoginButton,
    LogoutButton,
    isAuthenticated,
    userInfo,
  } from "../oidc/components.module"; // "@dopry/svelte-oidc";
</script>

<div class="row text-center align-items-center">
  <div class="col">
    <h1>
      <a class="navbar-brand" href="/">
        <img src={logo} alt="logo" />
        Stuff
      </a>
    </h1>
  </div>
  {#if $isAuthenticated}
    <div class="col">
      <LogoutButton>
        <span class="badge bg-primary">{$userInfo.name}</span> Logout
      </LogoutButton>
    </div>
  {:else}
    <div class="col">
      <LoginButton preserveRoute={true}>Login</LoginButton>
    </div>
  {/if}
</div>
