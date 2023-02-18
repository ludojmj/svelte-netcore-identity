<script>
  // StuffDelete.svelte
  import { navigate } from "svelte-navigator";
  import { crud } from "../lib/const.js";
  import { selectedItem } from "../lib/store.js";
  import { apiDeleteStuffAsync } from "../lib/api.js";
  import CommonForm from "./CommonForm.svelte";
  import Error from "./common/Error.svelte";
  export let id;

  $: stuffDatum = $selectedItem || {};

  const handleSubmit = async () => {
    stuffDatum = await apiDeleteStuffAsync(id);
    if (!stuffDatum.error) {
      navigate("/");
    }
  };
</script>

{#if id !== "" + $selectedItem.id}
  <Error msgErr="This is not what you want to delete." hasReset={true} />
{:else}
  <CommonForm
    title={crud.DELETE}
    {stuffDatum}
    inputError={null}
    disabled={true}
    {handleSubmit}
  />
{/if}
