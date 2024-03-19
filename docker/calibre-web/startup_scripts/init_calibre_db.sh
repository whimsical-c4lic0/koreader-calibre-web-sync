#!/bin/bash

if [ -d "/books/Sarah Louisa Forten Purvis" ]; then
    echo "poetry.epub already imported"
else
    echo "Adding poetry.epub to calibre database"
    /usr/bin/calibredb --with-library=/books add '/import/poetry.epub'
fi
