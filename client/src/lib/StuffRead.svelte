<script>
  // StuffRead.svelte
  import { navigate } from "svelte-navigator";
  import { selectedItem } from "../store.js";
  import CommonForm from "./CommonForm.svelte";
  import Error from "./Error.svelte";
  export let id;

  $: stuffDatum = $selectedItem || {};

  const handleCancel = () => {
    navigate("/");
  };
</script>

{#if id !== $selectedItem.id}
  <Error msgErr="This is not what you want to read." />
{:else}
  <CommonForm
    title="Reading a stuff"
    {stuffDatum}
    inputError={null}
    disabled={true}
    handleSubmit={handleCancel}
  />
{/if}
