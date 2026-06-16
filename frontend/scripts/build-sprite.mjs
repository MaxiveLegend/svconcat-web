import { readFile, writeFile, mkdir } from "node:fs/promises";
import { dirname, resolve } from "node:path";
import { fileURLToPath } from "node:url";
import icons from "../icons.config.js";

const here = dirname(fileURLToPath(import.meta.url));
const faSvgDir = resolve(here, "../node_modules/@fortawesome/fontawesome-free/svgs");
const outFile = resolve(here, "../public/images/icons/sprite.svg");

const VIEWBOX = /viewBox="([^"]+)"/;
const INNER = /<svg[^>]*>([\s\S]*?)<\/svg>/;
const COMMENT = /<!--[\s\S]*?-->/g;

async function toSymbol(id, faPath) {
    const raw = await readFile(resolve(faSvgDir, `${faPath}.svg`), "utf8");

    const viewBox = raw.match(VIEWBOX)?.[1];
    if (!viewBox) throw new Error(`No viewBox found for icon "${id}" (${faPath})`);

    const inner = raw.match(INNER)?.[1].replace(COMMENT, "").trim();
    if (!inner) throw new Error(`No inner markup found for icon "${id}" (${faPath})`);

    return `  <symbol id="${id}" viewBox="${viewBox}">${inner}</symbol>`;
}

const symbols = await Promise.all(
    Object.entries(icons).map(([id, faPath]) => toSymbol(id, faPath)),
);

const sprite = `<svg xmlns="http://www.w3.org/2000/svg" style="display:none">
${symbols.join("\n")}
</svg>
`;

await mkdir(dirname(outFile), { recursive: true });
await writeFile(outFile, sprite, "utf8");

console.log(`Built sprite with ${symbols.length} icon(s) -> ${outFile}`);
