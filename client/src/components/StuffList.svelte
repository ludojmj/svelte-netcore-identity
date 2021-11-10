<script>
  import { userInfo } from "../oidc/components.module"; // "@dopry/svelte-oidc";
  export let stuff;
  export let handleRead;
  export let handleUpdate;
  export let handleDelete;
</script>

<main>
  <table class="table table-striped table-hover" summary="List of stuff">
    <caption>.List of stuff</caption>
    <thead>
      <tr>
        <!-- <th scope="col">id</th> -->
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
          key={stuffDatum.id}
          class={stuffDatum.user.id === $userInfo.sub
            ? "table-success"
            : "table-danger"}
        >
          <!-- <td data-label="id">{datum.id}</td> -->
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
              value={stuffDatum.id}
              on:click={handleRead(stuffDatum.id)}
            >
              Read
            </button>
          </td>
          {#if stuffDatum.user.id === $userInfo.sub}
            <td>
              <button
                class="btn btn-warning"
                value={stuffDatum.id}
                on:click={() => handleUpdate(stuffDatum.id)}>Update</button
              >
            </td>
            <td>
              <button
                class="btn btn-danger"
                on:click={() => handleDelete(stuffDatum.id)}>Delete</button
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
