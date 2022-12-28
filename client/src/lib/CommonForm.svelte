<script>
  // CommonForm.svelte
  import { Link, navigate } from "svelte-navigator";
  import Error from "./Error.svelte";
  import Loading from "./Loading.svelte";
  export let title,
    stuffDatum,
    inputError,
    readonly,
    handleChange,
    handleSubmit;

  const init = (el) => {
    el.focus();
  };

  const handleEscape = (event) => {
    event.preventDefault();
    if (event.code == "Escape") {
      navigate("/");
    }
  };
</script>

<main>
  {#if !stuffDatum.error && !stuffDatum.id}
    <Loading />
  {/if}

  {#if stuffDatum.error}
    <Error msgErr={stuffDatum.error} />
    <Link class="btn btn-warning" to="/">Reset</Link>
  {:else if inputError}
    <Error msgErr={inputError} />
  {/if}

  {#if stuffDatum.id}
    <form
      class="alert alert-secondary"
      on:submit|preventDefault={handleSubmit}
      on:keyup={handleEscape}
    >
      <header
        class="modal-header alert {title.indexOf('Creating') > -1
          ? 'alert-success'
          : title.indexOf('Reading') > -1
          ? 'alert-dark'
          : title.indexOf('Updating') > -1
          ? 'alert-warning'
          : 'alert-danger'}"
      >
        <h5 class="modal-title">
          {title}
        </h5>
        <Link class="btn-close" to="/" />
      </header>

      <label class="form-label" for="user">Owner:</label>
      <input
        class="form-control"
        type="text"
        id="user"
        value={stuffDatum.user ? stuffDatum.user.givenName : "Current user"}
        aria-label="User"
        disabled={true}
        {readonly}
      />

      <label class="form-label" for="label">Label:</label>
      <input
        bind:value={stuffDatum.label}
        on:change={handleChange}
        use:init
        class="form-control"
        type="text"
        maxLength="79"
        name="label"
        id="label"
        placeholder="Label"
        {readonly}
      />

      <label class="form-label" for="description">Description:</label>
      <input
        bind:value={stuffDatum.description}
        on:change={handleChange}
        class="form-control"
        type="text"
        maxLength="79"
        name="description"
        id="description"
        placeholder="Description"
        {readonly}
      />

      <label class="form-label" for="otherInfo">Other info:</label>
      <textarea
        bind:value={stuffDatum.otherInfo}
        on:change={handleChange}
        class="form-control"
        rows="5"
        maxLength="399"
        name="otherInfo"
        id="otherInfo"
        placeholder="Other info"
        {readonly}
      />

      <footer>
        <Link class="btn btn-danger" to="/">Cancel</Link>
        <button class="btn btn-success" type="submit">Confirm</button>
      </footer>
    </form>
  {/if}
</main>
