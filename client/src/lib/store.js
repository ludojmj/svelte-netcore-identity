// lib/store.js
import { writable } from "svelte/store";

export const isAuthLoading = writable(false);
export const isLoading = writable(false);
export const selectedItem = writable(JSON.parse(localStorage.getItem("selectedItem")) || {});
export const tokens = writable(null);
