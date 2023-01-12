<script>
  // StuffCreate.svelte
  import { navigate } from "svelte-navigator";
  import { apiCreateStuff } from "../api/stuff";
  import CommonForm from "./CommonForm.svelte";

  let inputError = "";
  let stuffDatum = {
    id: "creating",
    label: "",
    description: "",
    otherInfo: "",
  };

  const handleSubmit = async () => {
    if (!/\S/.test(stuffDatum.label)) {
      inputError = "The label cannot be empty.";
      setTimeout(() => {
        inputError = "";
      }, 2000);

      return;
    }

    stuffDatum = await apiCreateStuff(stuffDatum);
    if (!stuffDatum.error) {
      navigate("/");
    }
  };
</script>

<main>
  <CommonForm
    title="Creating a stuff"
    {stuffDatum}
    {inputError}
    disabled={false}
    {handleSubmit}
  />
</main>
