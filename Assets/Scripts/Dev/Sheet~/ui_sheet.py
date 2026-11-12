"""
Builds the UI sheet from the screenshots of a run: one row per page, one column per resolution.

python3 ui_sheet.py <shots folder> <output.png> "<title>" [<page>="<row title>" ...]
Waits for the report of the run, <shots folder>/shots.txt, and prints it. When the run is done, builds the sheet
from the files <page>_<W>x<H>.png the report lists. Rows after the title pick pages, order them and set their
titles. Without them every page of the report gets a row with its name.
No report after the wait: the run still goes, so run the script again.
"""
import math
import os
import re
import sys
import time
from PIL import Image, ImageDraw, ImageFont

REPORT = "shots.txt"
WAIT_SECONDS = 100
ROW_HEIGHT = 540
MARGIN = 60
GAP = 40
BACKGROUND = (30, 30, 30)
TEXT = (255, 255, 255)
MUTED = (160, 160, 160)

here = os.path.dirname(os.path.abspath(__file__))
font_path = os.path.normpath(os.path.join(here, "../../../../Packages/net.perfectcore.perfectui/Fonts/Rubik/Rubik-VariableFont_wght.ttf"))


def font(size):
    return ImageFont.truetype(font_path, size)


def read_report(folder):
    path = os.path.join(folder, REPORT)
    deadline = time.time() + WAIT_SECONDS
    while not os.path.exists(path):
        if time.time() > deadline:
            sys.exit(f"No {path} after {WAIT_SECONDS} s: the run still goes, run this again")
        time.sleep(1)
    # The editor may still be writing the file.
    time.sleep(0.5)
    with open(path, encoding="utf-8") as file:
        return file.read()


def main():
    folder, output, title = sys.argv[1], sys.argv[2], sys.argv[3]
    report = read_report(folder)
    print(report)
    if not report.startswith("Shots done"):
        sys.exit(1)

    files = report.split("Files in", 1)[1].split(" Results:", 1)[0]
    shots = re.findall(r"([^,\s:]+)_(\d+)x(\d+)\.png", files)
    sizes = list(dict.fromkeys((int(w), int(h)) for _, w, h in shots))
    rows = [(arg.split("=", 1) * 2)[:2] for arg in sys.argv[4:]]
    if not rows:
        rows = [(page, page) for page in dict.fromkeys(page for page, _, _ in shots)]
    if not rows or not sizes:
        sys.exit("The report lists no screenshots of pages")

    widths = [round(w * ROW_HEIGHT / h) for w, h in sizes]
    width = MARGIN * 2 + sum(widths) + GAP * (len(widths) - 1)
    header = 170
    row_title = 70
    height = header + len(rows) * (row_title + ROW_HEIGHT + GAP) + MARGIN - GAP
    sheet = Image.new("RGB", (width, height), BACKGROUND)
    draw = ImageDraw.Draw(sheet)
    draw.text((MARGIN, 40), title, font=font(56), fill=TEXT)

    x = MARGIN
    for (w, h), shown in zip(sizes, widths):
        ratio = math.gcd(w, h)
        draw.text((x, 120), f"{w // ratio}:{h // ratio}  ·  {w}×{h}", font=font(30), fill=MUTED)
        x += shown + GAP

    y = header
    for prefix, caption in rows:
        draw.text((MARGIN, y + 10), caption, font=font(40), fill=TEXT)
        y += row_title
        x = MARGIN
        for (w, h), shown in zip(sizes, widths):
            path = os.path.join(folder, f"{prefix}_{w}x{h}.png")
            if os.path.exists(path):
                shot = Image.open(path).convert("RGB").resize((shown, ROW_HEIGHT), Image.LANCZOS)
                sheet.paste(shot, (x, y))
            else:
                draw.rectangle((x, y, x + shown, y + ROW_HEIGHT), outline=MUTED)
                draw.text((x + 20, y + 20), "missing", font=font(30), fill=MUTED)
            x += shown + GAP
        y += ROW_HEIGHT + GAP

    sheet.save(output)
    print(output, sheet.size)


if __name__ == "__main__":
    main()
