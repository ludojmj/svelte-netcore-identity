<script>
  // CommonForm.svelte
  import { Link, navigate } from "svelte-routing";
  import { crud } from "../lib/const.js";
  import Error from "./common/Error.svelte";
  import Loading from "./common/Loading.svelte";
  export let title, stuffDatum, inputError, disabled, handleSubmit;

  const setFocus = (el) => {
    el.focus();
  };

  const handleEscape = (event) => {
    if (event.code == "Escape") {
      navigate("/");
    }
  };
</script>

{#if !stuffDatum.error && !stuffDatum.id}
  <Loading />
{/if}

{#if stuffDatum.error}
  <Error msgErr={stuffDatum.error} hasReset={true} />
{:else if inputError}
  <Error msgErr={inputError} hasReset={false} />
{/if}

{#if stuffDatum.id}
  <form
    class="alert alert-secondary"
    on:submit|preventDefault={handleSubmit}
    on:keyup|preventDefault={handleEscape}
  >
    <header
      class="modal-header alert
        {(title === crud.CREATE && 'alert-success') ||
        (title === crud.READ && 'alert-dark') ||
        (title === crud.UPDATE && 'alert-warning') ||
        (title === crud.DELETE && 'alert-danger')}"
    >
      <h5 class="modal-title">
        {title}
      </h5>
      <Link class="btn-close" to="/" />
    </header>

    <label class="form-label" for="user">Owner:</label>
    <input
      class="form-control fw-bold"
      type="text"
      id="user"
      name="user"
      value={stuffDatum.user ? stuffDatum.user.givenName : "Current user"}
      aria-label="User"
      disabled
    />

    <label class="form-label" for="label">Label:</label>
    <input
      bind:value={stuffDatum.label}
      use:setFocus
      class="form-control"
      type="text"
      maxLength="79"
      placeholder="Label"
      {disabled}
    />

    <label class="form-label" for="description">Description:</label>
    <input
      bind:value={stuffDatum.description}
      class="form-control"
      type="text"
      maxLength="79"
      placeholder="Description"
      {disabled}
    />

    <label class="form-label" for="otherInfo">Other info:</label>
    <textarea
      bind:value={stuffDatum.otherInfo}
      class="form-control"
      rows="5"
      maxLength="399"
      placeholder="Other info"
      {disabled}
    />

    <footer>
      <Link class="btn btn-danger" to="/">Cancel</Link>
      <button class="btn btn-success" type="submit">Confirm</button>
    </footer>
  </form>
{/if}
