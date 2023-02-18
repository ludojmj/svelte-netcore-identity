<script>
  // StuffRead.svelte
  import { navigate } from "svelte-navigator";
  import { crud } from "../lib/const.js";
  import { selectedItem } from "../lib/store.js";
  import CommonForm from "./CommonForm.svelte";
  import Error from "./common/Error.svelte";
  export let id;

  $: stuffDatum = $selectedItem || {};

  const handleCancel = () => {
    navigate("/");
  };
</script>

{#if id !== $selectedItem.id}
  <Error msgErr="This is not what you want to read." hasReset={true} />
{:else}
  <CommonForm
    title={crud.READ}
    {stuffDatum}
    inputError={null}
    disabled={true}
    handleSubmit={handleCancel}
  />
{/if}
