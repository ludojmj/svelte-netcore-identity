<script>
  // AuthWrapper.svelte
  import { onMount } from "svelte";
  import { isAuthLoading, userInfo } from "../../lib/store.js";
  import { getTokenAync, getUserAsync } from "../../lib/oidc.js";
  import Loading from "../common/Loading.svelte";
  import Login from "./Login.svelte";

  onMount(async () => {
    await getTokenAync();
    await getUserAsync();
  });
</script>

<main class="container">
  <Login />
  {#if $isAuthLoading}
    <Loading />
  {:else if $userInfo}
    <slot />
    <pre>{JSON.stringify($userInfo, null, "\t")}</pre>
  {:else}
    <div class="alert alert-warning">
      You need to login to access this site.
    </div>
  {/if}
</main>
