<script>
  // Error.svelte
  import { Link } from "svelte-navigator";
  import { tokens } from "../../lib/store.js";
  import { apiErrMsg } from "../../lib/const.js";
  export let msgErr, hasReset;

  $: if (msgErr === apiErrMsg.unauthorized) tokens.set(null);
</script>

<div class="alert alert-danger row" role="alert">
  <div class="col-9">{msgErr}</div>
  {#if hasReset}
    <Link class="btn btn-warning" to="/">Reset</Link>
  {/if}
</div>
