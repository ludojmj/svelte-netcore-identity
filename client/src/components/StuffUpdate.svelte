<script>
  // StuffUpdate.svelte
  import { onMount } from "svelte";
  import { navigate } from "svelte-routing";
  import { crud } from "../lib/const.js";
  import { selectedItem } from "../lib/store.js";
  import { apiUpdateStuffAsync } from "../lib/api.js";
  import CommonForm from "./CommonForm.svelte";
  import Error from "./common/Error.svelte";
  export let id;

  $: stuffDatum = $selectedItem || {};
  const initialDatum = {};
  let inputError = "";

  onMount(async () => {
    for (let key in stuffDatum) {
      initialDatum[key] = stuffDatum[key];
    }
  });

  const handleSubmit = async () => {
    let hasChanged = false;
    for (let key in stuffDatum) {
      if (stuffDatum[key] !== initialDatum[key]) {
        hasChanged = true;
      }
    }

    if (!hasChanged) {
      inputError = "No significant changes...";
      setTimeout(() => {
        inputError = "";
      }, 2000);

      return;
    }

    if (!/\S/.test(stuffDatum.label)) {
      inputError = "The label cannot be empty.";
      setTimeout(() => {
        inputError = "";
      }, 2000);

      return;
    }

    stuffDatum = await apiUpdateStuffAsync(id, stuffDatum);
    if (!stuffDatum.error) {
      navigate("/");
    }
  };
</script>

{#if id !== "" + $selectedItem.id}
  <Error msgErr="This is not what you want to update." hasReset={true} />
{:else}
  <CommonForm
    title={crud.UPDATE}
    {stuffDatum}
    {inputError}
    disabled={false}
    {handleSubmit}
  />
{/if}
