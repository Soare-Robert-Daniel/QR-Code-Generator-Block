<?php
/*
Plugin Name: QR Code Generator
Plugin URI: http://URI_Of_Page_Describing_Plugin_and_Updates
Description: It generate a QR Code based on a giving text.
Version: 0.1.0
Author: Soare Robert Daniel 
Author URI: https://profiles.wordpress.org/soarerobertdaniel7/
License: GNU GPLv3 
*/

/**
 * Registers all block assets so that they can be enqueued through the block editor
 * in the corresponding context.
 *
 * @see https://developer.wordpress.org/block-editor/tutorials/block-tutorial/applying-styles-with-stylesheets/
 */
function create_block_testing_block_init() {
	$dir = dirname( __FILE__ );

	$script_asset_path = "$dir/build/index.asset.php";
	if ( ! file_exists( $script_asset_path ) ) {
		throw new Error(
			'You need to run `npm start` or `npm run build` for the "create-block/testing" block first.'
		);
	}
	$index_js     = 'build/index.js';
	$script_asset = require( $script_asset_path );
	wp_register_script(
		'create-block-testing-block-editor',
		plugins_url( $index_js, __FILE__ ),
		$script_asset['dependencies'],
		$script_asset['version']
	);

	$editor_css = 'build/index.css';
	wp_register_style(
		'create-block-testing-block-editor',
		plugins_url( $editor_css, __FILE__ ),
		array(),
		filemtime( "$dir/$editor_css" )
	);

	$style_css = 'build/style-index.css';
	wp_register_style(
		'create-block-testing-block',
		plugins_url( $style_css, __FILE__ ),
		array(),
		filemtime( "$dir/$style_css" )
	);

	register_block_type( 'create-block/testing', array(
		'editor_script' => 'create-block-testing-block-editor',
		'editor_style'  => 'create-block-testing-block-editor',
		'style'         => 'create-block-testing-block',
	) );
}
add_action( 'init', 'create_block_testing_block_init' );
