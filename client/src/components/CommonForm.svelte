<script>
  // CommonForm.svelte
  import Error from "./Error.svelte";
  import Loading from "./Loading.svelte";

  export let title,
    stuffDatum,
    inputError,
    disabled,
    handleChange,
    handleCancel,
    handleSubmit;

  const handleKeyDown = (event) => {
    if (event.keyCode === 13) {
      // ENTER
      return;
    }

    // ESCAPE
    if (event.keyCode === 27) {
      handleCancel();
    }
  };
</script>

<main>
  {#if !stuffDatum.id}
    {#if stuffDatum.error}
      <Error msgErr={stuffDatum.error} />
    {:else}
      <Loading />
    {/if}
  {/if}

  {#if inputError}
    <Error msgErr={inputError} />
  {/if}

  {#if stuffDatum.id}
    <form
      on:submit|preventDefault={handleSubmit}
      on:keydown={handleKeyDown}
      class="alert alert-secondary"
      value={stuffDatum}
    >
      <!-- Prevent implicit submission of the form -->
      <button class="d-none" type="submit" disabled aria-hidden="true" />

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
        <button type="button" class="btn-close" on:click={handleCancel} />
      </header>

      <label class="form-label" for="user">Owner:</label>
      <input
        class="form-control"
        type="text"
        id="user"
        value={stuffDatum.user ? stuffDatum.user.givenName : "Current user"}
        aria-label="User"
        disabled
        readonly
      />

      <label class="form-label" for="label">Label:</label>
      <input
        class="form-control"
        type="text"
        maxLength="79"
        name="label"
        id="label"
        placeholder="Label"
        value={stuffDatum.label}
        on:change={handleChange}
        {disabled}
      />

      <label class="form-label" for="description">Description:</label>
      <input
        class="form-control"
        type="text"
        maxLength="79"
        name="description"
        id="description"
        placeholder="Description"
        value={stuffDatum.description}
        on:change={handleChange}
        {disabled}
      />

      <label class="form-label" for="otherInfo">Other info:</label>
      <textarea
        class="form-control"
        rows="5"
        maxLength="399"
        name="otherInfo"
        id="otherInfo"
        placeholder="Other info"
        value={stuffDatum.otherInfo}
        on:change={handleChange}
        {disabled}
      />

      <footer>
        <button class="btn btn-danger" on:click={handleCancel}>Cancel</button>
        <button class="btn btn-success" type="submit">Confirm</button>
      </footer>
    </form>
  {/if}
</main>
