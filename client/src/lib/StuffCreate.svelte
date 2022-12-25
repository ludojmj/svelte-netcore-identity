<script>
  // StuffCreate.svelte
  import { onMount } from "svelte";
  import { useNavigate } from "svelte-navigator";
  import { apiCreateStuff } from "../api/stuff";
  import { accessToken, idToken } from "../oidc/components.module"; // "@dopry/svelte-oidc";
  import CommonForm from "./CommonForm.svelte";

  let inputError = "";
  const initialDatum = {
    id: "creating",
    label: "",
    description: "",
    otherInfo: "",
  };
  let stuffDatum = {};

  onMount(() => {
    stuffDatum = initialDatum;
  });

  const handleChange = (event) => {
    const { name, value } = event.target;
    stuffDatum = { ...stuffDatum, [name]: value };
    inputError = "";
  };

  const navigate = useNavigate();
  const handleSubmit = async (event) => {
    const formData = new FormData(event.target);
    for (let field of formData) {
      const [key, value] = field;
      stuffDatum[key] = value;
    }

    if (!/\S/.test(stuffDatum.label)) {
      inputError = "The label cannot be empty.";
      setTimeout(() => {
        inputError = "";
      }, 2000);

      return;
    }

    stuffDatum = await apiCreateStuff(stuffDatum, $accessToken, $idToken);
    if (stuffDatum.error) {
      inputError = stuffDatum.error;
    } else {
      navigate("/");
    }
  };
</script>

<main>
  <CommonForm
    title="Creating a stuff"
    stuffDatum={initialDatum}
    {inputError}
    readonly={false}
    {handleChange}
    {handleSubmit}
  />
</main>
