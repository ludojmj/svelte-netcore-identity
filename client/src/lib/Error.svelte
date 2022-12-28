<script>
  // Error.svelte
  export let msgErr;
</script>

<main>
  <div class="alert alert-danger row" role="alert">
    <div class="col-9">{msgErr}</div>
  </div>
</main>
