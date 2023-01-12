<script>
  // StuffDelete.svelte
  import { navigate } from "svelte-navigator";
  import { selectedItem } from "../store.js";
  import { apiDeleteStuff } from "../api/stuff";
  import CommonForm from "./CommonForm.svelte";
  import Error from "./Error.svelte";
  export let id;

  $: stuffDatum = $selectedItem || {};

  const handleSubmit = async () => {
    stuffDatum = await apiDeleteStuff(id);
    if (!stuffDatum.error) {
      navigate("/");
    }
  };
</script>

{#if id !== "" + $selectedItem.id}
  <Error msgErr="This is not what you want to delete." />
{:else}
  <CommonForm
    title="Deleting a stuff"
    {stuffDatum}
    inputError={null}
    disabled={true}
    {handleSubmit}
  />
{/if}
