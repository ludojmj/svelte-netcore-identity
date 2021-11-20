<script>
  // StuffList.svelte
  import { useNavigate } from "svelte-navigator";
  import { userInfo } from "../oidc/components.module"; // "@dopry/svelte-oidc";
  export let stuff;

  const navigate = useNavigate();
  const handleRead = (event) => {
    navigate("read/" + event.currentTarget.value);
  };

  const handleUpdate = (event) => {
    navigate("update/" + event.currentTarget.value);
  };

  const handleDelete = (event) => {
    navigate("delete/" + event.currentTarget.value);
  };
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
              on:click={handleRead}
            >
              Read
            </button>
          </td>
          {#if stuffDatum.user.id === $userInfo.sub}
            <td>
              <button
                class="btn btn-warning"
                value={stuffDatum.id}
                on:click={handleUpdate}>Update</button
              >
            </td>
            <td>
              <button
                class="btn btn-danger"
                value={stuffDatum.id}
                on:click={handleDelete}>Delete</button
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
