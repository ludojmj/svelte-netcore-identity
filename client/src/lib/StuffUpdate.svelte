<script>
  // StuffUpdate.svelte
  import { onMount } from "svelte";
  import { navigate } from "svelte-navigator";
  import { selectedItem } from "../store.js";
  import { apiUpdateStuff } from "../api/stuff";
  import { accessToken, idToken } from "../oidc/components.module"; // "@dopry/svelte-oidc";
  import CommonForm from "./CommonForm.svelte";
  import Error from "./Error.svelte";
  export let id;

  $: stuffDatum = $selectedItem || {};
  const initialDatum = {};
  let inputError = "";

  onMount(async () => {
    for (let key in stuffDatum) {
      initialDatum[key] = stuffDatum[key];
    }
  });

  const handleChange = (event) => {
    inputError = "";
    const { name, value } = event.target;
    stuffDatum = { ...stuffDatum, [name]: value };
  };

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

    stuffDatum = await apiUpdateStuff(id, stuffDatum, $accessToken, $idToken);
    if (!stuffDatum.error) {
      navigate("/");
    }
  };
</script>

{#if id !== "" + $selectedItem.id}
  <Error msgErr="This is not what you want to update." />
{:else}
  <CommonForm
    title="Updating a stuff"
    {stuffDatum}
    {inputError}
    readonly={false}
    {handleChange}
    {handleSubmit}
  />
{/if}
