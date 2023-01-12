<script>
  // Header.svelte
  import Login from "./oidc/Login.svelte";
  import Logout from "./oidc/Logout.svelte";
  let logo = "/logo.svg";
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

  <div class="col">
    <Logout />
  </div>

  <div class="col">
    <Login />
  </div>
</div>
