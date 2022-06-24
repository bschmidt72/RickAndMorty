<template>
<div class="search-container" v-on:mouseover="mouseOver = true" v-on:mouseleave="mouseOver = false">
  <h5>Search a character</h5>
  <input name="search" placeholder="Search" v-model="searchInput" autocomplete="off" v-on:keyup.enter="sendResults()">
    <ul v-if="suggestions?.length > 0 && suggestionsOpen" class="suggestions styled-scrollbars">
      <li v-for="suggestion of suggestions" class="suggestion" @click="select(suggestion)">{{suggestion.name}}</li>
    </ul>
</div> 
</template>

<script setup lang="ts">
import {onMounted, onBeforeUnmount, ref, watch} from "vue";
import type {Ref} from 'vue';
import type {CharacterInfo} from "@/model/CharacterInfo";
import {RickAndMortyService} from "@/services/RickAndMortyService";

const emit = defineEmits<{
  (e: 'characterSelected', character: CharacterInfo): void,
  (e: 'searchResults', searchResults: CharacterInfo[]): void
}>()

const service = new RickAndMortyService();
const searchInput = ref('');
const suggestions = ref([] as CharacterInfo[]);
const selected: Ref<CharacterInfo | null> = ref(null);
const abortQuery: Ref<AbortController | null> = ref(null);
const suggestionsOpen = ref(false);
const mouseOver = ref(false);

onMounted( () => {
  setTimeout(() => document.addEventListener('click', hide));
});
onBeforeUnmount(() => document.removeEventListener('click', hide));

watch(searchInput, async (newInput, oldInput) => {
  if (newInput === selected.value?.name) {
    return;
  }
  if (!newInput || newInput.length === 0) {
    suggestions.value = [];
    return;
  }
  query(newInput);
})

function hide() {
  suggestionsOpen.value = false;
}

function query(search: string) {
  abortQuery.value?.abort();
  let controller = new AbortController();
  abortQuery.value = controller;
  service.queryCharacterNames(search, controller)
      .then(result => {
        suggestions.value = result;
        suggestionsOpen.value = true
      });
}

function select(characterInfo: CharacterInfo) {
  selected.value = characterInfo;
  searchInput.value = characterInfo.name;
  suggestions.value = [ characterInfo ];
  suggestionsOpen.value = false;
  emit('characterSelected', characterInfo)
}

function sendResults() {
  suggestionsOpen.value = false;
  emit('searchResults', suggestions.value);
}

</script>

<style scoped>

.search-container {
  display: flex;
  flex: 1 1 auto;
  flex-direction: column;
  border: 0;
}
input {
  color: rgba(255, 255, 255, 0.75);
  font-size: 13pt;
  font-weight: normal;
  line-height: 1.75;
  background-color: #b74e91;
  appearance: none;
  border-radius: 0.25em;
  border: solid 1px rgba(255, 255, 255, 0.15);
  display: block;
  outline: 0;
  padding: 0 1em;
  text-decoration: none;
  width: 300px;
  max-width: calc(100% - 1em) !important;
}
::placeholder {
  color: rgba(255, 255, 255, 0.55);
}

.suggestions {
  flex: 1 1 100%;
  margin: 0;
  min-height: 20px;
  max-height: 620px;
  overflow: auto;
  max-width: 300px;
  padding: 0 1em;
}
.suggestion {
  margin: 0 1em;
  flex: 1 1 100%;
  list-style: none;
  text-align: left;
  padding: 4px 2px;
  cursor: pointer;
  word-break: break-all;
  max-width: 300px;
}
.suggestion:hover {
  background-color: #b74e91;
  opacity: 0.5;
  color: white;
}
@media screen and (max-width: 800px) {
  .search-container {
    width: calc(100% - 2em);
    min-width: calc(100% - 2em);
  }
  .suggestions {
    flex: 1 1 auto;
    margin: 0;
    min-height: 20px;
    max-height: 150px;
    overflow: auto;
    max-width: calc(100% - 1em);
    padding: 0 1em;
  }
  input {
    font-size: 11pt;
    max-width: calc(100% - 1em);
  }
}
</style>