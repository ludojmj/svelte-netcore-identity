<script>
  // CrudManager.svelte
  import { onMount } from "svelte";
  import { useNavigate } from "svelte-navigator";
  import { accessToken, idToken } from "../oidc/components.module"; // "@dopry/svelte-oidc";
  import { apiGetStuffList, apiSearchStuff, apiGotoPage } from "../api/stuff";
  import StuffList from "./StuffList.svelte";
  import Error from "./Error.svelte";
  import Loading from "./Loading.svelte";

  let search = { filter: "" };
  let stuff = {};
  onMount(async () => {
    stuff = await apiGetStuffList($accessToken, $idToken);
  });

  const handleReset = async () => {
    search = { filter: "" };
    stuff = await apiGetStuffList($accessToken, $idToken);
  };

  const handleSearch = async () => {
    if (search.filter.length) {
      stuff = await apiSearchStuff(search.filter, $accessToken, $idToken);
      return;
    }

    handleReset();
  };

  const handlePage = async (event) => {
    const page =
      event.currentTarget.value === "+" ? stuff.page + 1 : stuff.page - 1;
    stuff = await apiGotoPage(page, $accessToken, $idToken);

    search = { filter: "" };
  };

  const handleChangeFilter = (event) => {
    const { name, value } = event.target;
    search = { ...search, [name]: value };
  };

  const navigate = useNavigate();
  const handleCreate = () => {
    navigate("/create");
  };
</script>

<main>
  {#if !stuff.page}
    {#if stuff.error}
      <Error msgErr={stuff.error} />
    {:else}
      <Loading />
    {/if}
  {/if}

  {#if stuff.page}
    <form on:submit|preventDefault={handleSearch}>
      <table class="table text-end" summary="Menu">
        <tbody>
          <tr>
            <td>
              <div class="input-group">
                <input
                  class="form-control"
                  id="filter"
                  name="filter"
                  type="text"
                  placeholder="Filter"
                  aria-label="Filter"
                  maxLength="20"
                  value={search.filter}
                  on:change={handleChangeFilter}
                />
                <button class="btn btn-primary" type="submit"> Search </button>
              </div>
            </td>
            <td>
              <button class="btn btn-info" on:click={handleReset}>Reset</button>
            </td>
            <td>
              <ul class="pagination justify-content-end">
                <li class="page-item">
                  <button
                    class="btn btn-primary"
                    value="-"
                    on:click|preventDefault={handlePage}
                    disabled={stuff.page === 1}
                  >
                    &laquo;
                  </button>
                </li>
                <li class="page-item">
                  <div class="form-control">
                    Page {stuff.page}/{stuff.totalPages}
                  </div>
                </li>
                <li class="page-item">
                  <button
                    class="btn btn-primary"
                    value="+"
                    on:click|preventDefault={handlePage}
                    disabled={stuff.page === stuff.totalPages}
                  >
                    &raquo;
                  </button>
                </li>
              </ul>
            </td>
            <td>
              <button class="btn btn-success" on:click={handleCreate}>
                Create
              </button>
            </td>
          </tr>
        </tbody>
      </table>
    </form>
    <StuffList {stuff} />
  {/if}
</main>
