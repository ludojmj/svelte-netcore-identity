<script>
  // StuffList.svelte
  import { VanillaOidc } from "@axa-fr/vanilla-oidc";
  import { navigate } from "svelte-navigator";
  import { selectedItem, tokens } from "../store.js";
  import { configuration } from "../oidcConf.js";
  export let stuff;

  const handleRead = (stuffDatum) => {
    selectedItem.update(() => stuffDatum);
    navigate("/read/" + stuffDatum.id);
  };

  const handleUpdate = (stuffDatum) => {
    selectedItem.update(() => stuffDatum);
    navigate("/update/" + stuffDatum.id);
  };

  const handleDelete = (stuffDatum) => {
    selectedItem.update(() => stuffDatum);
    navigate("/delete/" + stuffDatum.id);
  };

  const vanillaOidc = VanillaOidc.getOrCreate(configuration);
</script>

<main>
  <table class="table table-striped table-hover" summary="List of stuff">
    <caption>.List of stuff</caption>
    <thead>
      <tr>
        <th scope="col">Label</th>
        <th scope="col">Description</th>
        <th scope="col">CreatedAt</th>
        <th scope="col">UpdatedAt</th>
        <th colspan="3" />
      </tr>
    </thead>
    <tbody>
      {#each stuff.datumList as stuffDatum}
        <tr
          id={stuffDatum.id}
          class={stuffDatum.user.id === $tokens.idTokenPayload.sub
            ? "table-success"
            : "table-danger"}
        >
          <td data-label="Label">{stuffDatum.label}</td>
          <td data-label="Description">{stuffDatum.description}</td>
          <td data-label="createdAt">
            <code>
              {#if stuffDatum.createdAt}
                {new Date(stuffDatum.createdAt).toLocaleString()}
              {:else}
                -
              {/if}
            </code>
          </td>
          <td data-label="updatedAt">
            <code>
              {#if stuffDatum.updatedAt}
                {new Date(stuffDatum.updatedAt).toLocaleString()}
              {:else}
                -
              {/if}
            </code>
          </td>
          <td>
            <button
              class="btn btn-secondary"
              on:click={() => handleRead(stuffDatum)}
            >
              Read
            </button>
          </td>
          {#if stuffDatum.user.id === $tokens.idTokenPayload.sub}
            <td>
              <button
                class="btn btn-warning"
                on:click={() => handleUpdate(stuffDatum)}>Update</button
              >
            </td>
            <td>
              <button
                class="btn btn-danger"
                on:click={() => handleDelete(stuffDatum)}>Delete</button
              >
            </td>
          {:else}
            <td colspan="2">
              Owned by:
              <code>
                {stuffDatum.user.givenName}
                {stuffDatum.user.familyName}
              </code>
            </td>
          {/if}
        </tr>
      {/each}
    </tbody>
  </table>
</main>
