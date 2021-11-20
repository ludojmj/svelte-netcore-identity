<script>
  // StuffRead.svelte
  import { useNavigate } from "svelte-navigator";
  import { selectedItem } from "../store.js";
  import CommonForm from "./CommonForm.svelte";
  import Error from "./Error.svelte";
  export let id;

  $: stuffDatum = $selectedItem || {};
  const navigate = useNavigate();
  const handleCancel = () => {
    navigate("/");
  };
</script>

<main>
  {#if id !== $selectedItem.id}
    <Error msgErr="This is not what you want to read." />
  {:else}
    <CommonForm
      title="Reading a stuff"
      {stuffDatum}
      inputError={null}
      readonly={true}
      handleChange={null}
      {handleCancel}
      handleSubmit={handleCancel}
    />
  {/if}
</main>
