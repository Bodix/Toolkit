"""
Builds a UI sheet from UiShots screenshots: one row per element, one column per resolution.

python3 ui_sheet.py <shots folder> <output.png> "<title>" <prefix>="<row title>" ...
Each row uses <shots folder>/<prefix>_<W>x<H>.png for every size in SIZES.
"""
import os
import sys
from PIL import Image, ImageDraw, ImageFont

SIZES = [("7:3", 2520, 1080), ("4:3", 1440, 1080)]
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


def main():
    folder, output, title = sys.argv[1], sys.argv[2], sys.argv[3]
    rows = [arg.split("=", 1) for arg in sys.argv[4:]]
    widths = [round(w * ROW_HEIGHT / h) for _, w, h in SIZES]
    width = MARGIN * 2 + sum(widths) + GAP * (len(widths) - 1)
    header = 170
    row_title = 70
    height = header + len(rows) * (row_title + ROW_HEIGHT + GAP) + MARGIN - GAP
    sheet = Image.new("RGB", (width, height), BACKGROUND)
    draw = ImageDraw.Draw(sheet)
    draw.text((MARGIN, 40), title, font=font(56), fill=TEXT)

    x = MARGIN
    for (name, w, h), shown in zip(SIZES, widths):
        draw.text((x, 120), f"{name}  ·  {w}×{h}", font=font(30), fill=MUTED)
        x += shown + GAP

    y = header
    for prefix, caption in rows:
        draw.text((MARGIN, y + 10), caption, font=font(40), fill=TEXT)
        y += row_title
        x = MARGIN
        for (_, w, h), shown in zip(SIZES, widths):
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
